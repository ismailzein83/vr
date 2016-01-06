using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        T _emptyObject;

        static int s_maxRetryDequeueTime;

        internal PersistentQueue(int queueId, QueueSettings settings)
        {
            _dataManagerQueue = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            _dataManagerQueueItem = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
            queueSubscriptionManager = new QueueSubscriptionManager();
            _runningProcessManager = new RunningProcessManager();
            _emptyObject = Activator.CreateInstance<T>();
            _queueId = queueId;
            _queueSettings = settings;
            if (!int.TryParse(ConfigurationManager.AppSettings["Queue_MaxRetryDequeueTime"], out s_maxRetryDequeueTime))
                s_maxRetryDequeueTime = 5;
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
            string itemDescription = item.GenerateDescription();
            byte[] serialized = item.Serialize();
            byte[] compressed = Vanrise.Common.Compressor.Compress(serialized);

            long itemId = _dataManagerQueueItem.GenerateItemID();
            long executionFlowTriggerItemId = item.ExecutionFlowTriggerItemId > 0 ? item.ExecutionFlowTriggerItemId : itemId;//if first item in the flow, assign the item id to the ExecutionFlowTriggerItemId

            var subscribedQueueIds = queueSubscriptionManager.GetSubscribedQueueIds(_queueId);
            if (subscribedQueueIds == null || subscribedQueueIds.Count == 0)
            {
                _dataManagerQueueItem.EnqueueItem(_queueId, itemId, executionFlowTriggerItemId, compressed, itemDescription, QueueItemStatus.New);
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
                _dataManagerQueueItem.EnqueueItem(targetQueuesItemsIds, _queueId, itemId, executionFlowTriggerItemId, compressed, itemDescription, QueueItemStatus.New);
            }
            return itemId;
        }

        public override bool TryDequeue(Action<T> processItem)
        {
            if (processItem == null)
                throw new ArgumentNullException("processItem");
            if (this.IsListening)
                throw new InvalidOperationException("TryDequeue cannot be called while this Queue is listening");

            return TryDequeuePrivate(processItem, true);
        }

        #region Disabled Functionalities

        //public void StartListening(Action<T> processItem, int maxThreads = 1)
        //{
        //    if (processItem == null)
        //        throw new ArgumentNullException("processItem");
            
        //    lock (this)
        //    {
        //        if (this.IsListening)
        //            throw new InvalidOperationException("Listening is already started on this queue");
        //        this.IsListening = true;
        //    }

        //    Task task = new Task(() =>
        //    {
        //        int consecutiveFoundItems = 0;
        //        do
        //        {
        //            if (TryDequeuePrivate(processItem, false))//if item is returned from the queue
        //                consecutiveFoundItems++;
        //            else
        //            {
        //                if (consecutiveFoundItems > 0)
        //                    consecutiveFoundItems--;
        //                Thread.Sleep(1000);
        //            }

        //            //if many items are found in the queue consecutively, initialize multiple concurrent threads to dequeue and process items
        //            if (consecutiveFoundItems > 1 && maxThreads > 1)
        //            {
        //                Parallel.For(0, maxThreads, (i) =>
        //                {
        //                    bool hasItem = false;
        //                    do
        //                    {
        //                        hasItem = TryDequeuePrivate(processItem, false);
        //                    }
        //                    while (this.IsListening && hasItem);
        //                });
        //            }
        //        }
        //        while (this.IsListening);
        //    });
        //    task.Start();
        //}

        //public void StopListening()
        //{
        //    lock (this)
        //        this.IsListening = false;
        //}

        #endregion

        #endregion

        #region Private Methods

        private bool TryDequeuePrivate(Action<T> onItemReady, bool rethrowOnError)
        {
            int currentProcessId = RunningProcessManager.CurrentProcess.ProcessId;
            IEnumerable<int> runningProcessesIds = _runningProcessManager.GetCachedRunningProcesses(new TimeSpan(0, 0, 15)).Select(itm => itm.ProcessId);
            QueueItem queueItem = _dataManagerQueueItem.DequeueItem(_queueId, currentProcessId, runningProcessesIds, _queueSettings.SingleConcurrentReader);
            if (queueItem != null)
            {
                _dataManagerQueueItem.UpdateHeaderStatus(queueItem.ItemId, QueueItemStatus.Processing);
                byte[] decompressed = Vanrise.Common.Compressor.Decompress(queueItem.Content);
                T deserialized = _emptyObject.Deserialize<T>(decompressed);
                deserialized.ExecutionFlowTriggerItemId = queueItem.ExecutionFlowTriggerItemId;
                try
                {
                    onItemReady(deserialized);
                    _dataManagerQueueItem.DeleteItem(_queueId, queueItem.ItemId);
                    _dataManagerQueueItem.UpdateHeaderStatus(queueItem.ItemId, QueueItemStatus.Processed);
                }
                catch (Exception ex)
                {
                    QueueItemHeader itemHeader = _dataManagerQueueItem.GetHeader(queueItem.ItemId, _queueId);
                    QueueItemStatus failedStatus = QueueItemStatus.Failed;
                    int retryCount = itemHeader.RetryCount + 1;
                    if (retryCount >= s_maxRetryDequeueTime)
                        failedStatus = QueueItemStatus.Suspended;
                    _dataManagerQueueItem.UnlockItem(_queueId, queueItem.ItemId, (failedStatus == QueueItemStatus.Suspended));
                    _dataManagerQueueItem.UpdateHeader(queueItem.ItemId, failedStatus, retryCount, ex.ToString());
                    if (rethrowOnError)
                        throw;
                    else
                        return false;
                }
                return true;
            }
            else
                return false;
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


        public bool TryDequeueObject(Action<PersistentQueueItem> processItem)
        {
            return this.TryDequeue(processItem);
        }

        #endregion


    }
}
