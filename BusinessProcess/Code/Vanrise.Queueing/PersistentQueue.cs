using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vanrise.Entities.SummaryTransformation;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Vanrise.Queueing
{    
    public class PersistentQueue<T> : BaseQueue<T>, IPersistentQueue where T : PersistentQueueItem
    {
        #region ctor/Fields

        IQueueDataManager _dataManagerQueue;
        IQueueItemDataManager _dataManagerQueueItem;

        QueueSubscriptionManager queueSubscriptionManager;

        RunningProcessManager _runningProcessManager;
        int _queueId;
        QueueSettings _queueSettings;

        public QueueSettings QueueSettings
        {
            get
            {
                return _queueSettings;
            }
        }

        T _emptyObject;

        int _maxRetryDequeueTime;
        int _nbOfSummaryBatchesToDequeue;

        internal PersistentQueue(int queueId, QueueSettings settings)
        {
            _dataManagerQueue = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            _dataManagerQueueItem = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
            queueSubscriptionManager = new QueueSubscriptionManager();
            _runningProcessManager = new RunningProcessManager();
            _emptyObject = Activator.CreateInstance<T>();
            _queueId = queueId;
            _queueSettings = settings;
            if (!int.TryParse(ConfigurationManager.AppSettings["Queue_MaxRetryDequeueTime"], out _maxRetryDequeueTime))
                _maxRetryDequeueTime = 5;
            if (!int.TryParse(ConfigurationManager.AppSettings["Queue_NbOfSummaryBatchesToDequeue"], out _nbOfSummaryBatchesToDequeue))
                _nbOfSummaryBatchesToDequeue = 20;
        }

        #endregion

        #region Properties

        public bool IsListening { get; private set; }

        #endregion

        #region Public Methods
       
        public override void Enqueue(T item)
        {
            EnqueuePrivate(item);
        }

        long EnqueuePrivate(T item)
        {
            DateTime? batchStart = item.BatchStart != DateTime.MinValue ? item.BatchStart : default(DateTime?);            
            string itemDescription = item.GenerateDescription();
            byte[] serialized = item.Serialize();
            byte[] compressed = Vanrise.Common.Compressor.Compress(serialized);

            long itemId = _dataManagerQueueItem.GenerateItemID();
            long executionFlowTriggerItemId = item.ExecutionFlowTriggerItemId > 0 ? item.ExecutionFlowTriggerItemId : itemId;//if first item in the flow, assign the item id to the ExecutionFlowTriggerItemId

            var subscribedQueueIds = queueSubscriptionManager.GetSubscribedQueueIds(_queueId);
            if (subscribedQueueIds == null || subscribedQueueIds.Count == 0)
            {
                _dataManagerQueueItem.EnqueueItem(_queueId, itemId, batchStart, executionFlowTriggerItemId, compressed, itemDescription, QueueItemStatus.New);
            }
            else
            {
                Dictionary<int, long> targetQueuesItemsIds = new Dictionary<int, long>();
                targetQueuesItemsIds.Add(_queueId, itemId);
                if (subscribedQueueIds != null)
                {
                    foreach (int queueId in subscribedQueueIds)
                        targetQueuesItemsIds.Add(queueId, _dataManagerQueueItem.GenerateItemID());
                }
                _dataManagerQueueItem.EnqueueItem(targetQueuesItemsIds, _queueId, itemId, batchStart, executionFlowTriggerItemId, compressed, itemDescription, QueueItemStatus.New);
            }
            return itemId;
        }

        public override bool TryDequeue(Action<T> processItem)
        {
            if (processItem == null)
                throw new ArgumentNullException("processItem");

            return TryDequeuePrivate(processItem, null);
        }

        #endregion

        #region Private Methods

        private bool TryDequeuePrivate(Action<T> processItem, IPersistentQueueDequeueContext context)
        {
            QueueItem queueItem = null;
            if (context != null && context.ActivatorInstanceId.HasValue)
            {
                queueItem = _dataManagerQueueItem.DequeueItem(_queueId, context.ActivatorInstanceId.Value);
            }
            else
            {
                int currentProcessId = RunningProcessManager.CurrentProcess.ProcessId;
                IEnumerable<int> runningProcessesIds = _runningProcessManager.GetCachedRunningProcesses().Select(itm => itm.ProcessId);
                queueItem = _dataManagerQueueItem.DequeueItem(_queueId, currentProcessId, runningProcessesIds, _queueSettings.MaximumConcurrentReaders);
            }
            if (queueItem != null)
            {
                _dataManagerQueueItem.UpdateHeaderStatus(queueItem.ItemId, QueueItemStatus.Processing);
                T deserialized = DeserializeQueueItem(queueItem);
                try
                {
                    processItem(deserialized);
                    _dataManagerQueueItem.DeleteItem(_queueId, queueItem.ItemId);
                    _dataManagerQueueItem.UpdateHeaderStatus(queueItem.ItemId, QueueItemStatus.Processed);
                }
                catch (Exception ex)
                {
                    QueueItemHeader itemHeader = _dataManagerQueueItem.GetHeader(queueItem.ItemId, _queueId);
                    QueueItemStatus failedStatus = QueueItemStatus.Failed;
                    int retryCount = itemHeader.RetryCount + 1;
                    if (retryCount >= _maxRetryDequeueTime)
                        failedStatus = QueueItemStatus.Suspended;
                    _dataManagerQueueItem.UpdateHeader(queueItem.ItemId, failedStatus, retryCount, ex.ToString());
                    _dataManagerQueueItem.UnlockItem(_queueId, queueItem.ItemId, (failedStatus == QueueItemStatus.Suspended));
                    throw;
                }
                return true;
            }
            else
                return false;
        }

        private T DeserializeQueueItem(QueueItem queueItem)
        {
            byte[] decompressed = Vanrise.Common.Compressor.Decompress(queueItem.Content);
            T deserialized = _emptyObject.Deserialize<T>(decompressed);
            deserialized.ExecutionFlowTriggerItemId = queueItem.ExecutionFlowTriggerItemId;
            deserialized.BatchStart = queueItem.BatchStart;
            return deserialized;
        }

        #endregion

        #region IPersistentQueue

        public long EnqueueObject(PersistentQueueItem item)
        {
            T typedItem = item as T;
            if (typedItem == null)
                throw new Exception(String.Format("item is not of type '{0}'", typeof(T)));
            return this.EnqueuePrivate(typedItem);
        }


        public bool TryDequeueObject(Action<PersistentQueueItem> processItem, IPersistentQueueDequeueContext context)
        {
            if (processItem == null)
                throw new ArgumentNullException("processItem");
            return this.TryDequeuePrivate(processItem, context);
        }

        #endregion

        public bool TryDequeueSummaryBatches(DateTime batchStart, Action<IEnumerable<PersistentQueueItem>> processBatches)
        {
            IEnumerable<QueueItem> summaryBatches = _dataManagerQueueItem.DequeueSummaryBatches(_queueId, batchStart, _nbOfSummaryBatchesToDequeue);
            if (summaryBatches != null && summaryBatches.Count() > 0)
            {
                var itemsIds = summaryBatches.Select(itm => itm.ItemId);
                _dataManagerQueueItem.UpdateHeaderStatuses(itemsIds, QueueItemStatus.Processing);
                List<PersistentQueueItem> deserializedBatches = new List<PersistentQueueItem>();
                foreach(var summaryBatch in summaryBatches)
                {
                    T deserialized = DeserializeQueueItem(summaryBatch);
                    deserializedBatches.Add(deserialized);
                }
                
                try
                {
                    processBatches(deserializedBatches);
                    _dataManagerQueueItem.DeleteItems(_queueId, itemsIds);
                    _dataManagerQueueItem.UpdateHeaderStatuses(itemsIds, QueueItemStatus.Processed);
                }
                catch (Exception ex)
                {
                    QueueItemHeader itemHeader = _dataManagerQueueItem.GetHeader(itemsIds.First(), _queueId);
                    QueueItemStatus failedStatus = QueueItemStatus.Failed;
                    int retryCount = itemHeader.RetryCount + 1;
                    if (retryCount >= _maxRetryDequeueTime)
                        failedStatus = QueueItemStatus.Suspended;
                    _dataManagerQueueItem.UpdateHeaders(itemsIds, failedStatus, retryCount, ex.ToString());
                    if(failedStatus == QueueItemStatus.Suspended)
                        _dataManagerQueueItem.SetItemsSuspended(_queueId, itemsIds);
                    throw;
                }
                return true;
            }
            else
                return false;
        }
    }


}
