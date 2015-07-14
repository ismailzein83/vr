using System;
using System.Collections.Generic;
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

        RunningProcessManager _runningProcessManager;
        int _queueId;
        QueueSettings _queueSettings;
        List<int> _subscribedQueueIds;
        T _emptyObject;

        internal PersistentQueue(int queueId, QueueSettings settings)
        {
            _dataManagerQueue = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            _dataManagerQueueItem = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
            _runningProcessManager = new RunningProcessManager();
            _emptyObject = Activator.CreateInstance<T>();
            _queueId = queueId;
            _queueSettings = settings;
        }

        #endregion

        #region Properties

        public bool IsListening { get; private set; }

        #endregion

        #region Public Methods
       
        public override void Enqueue(T item)
        {
            UpdateSubscribedQueueIdsIfNeeded();
            string itemDescription = item.GenerateDescription();
            byte[] serialized = item.Serialize();
            byte[] compressed = Vanrise.Common.Compressor.Compress(serialized);

            long itemId = _dataManagerQueueItem.GenerateItemID(_queueId);
            if (_subscribedQueueIds == null || _subscribedQueueIds.Count == 0)
            {
                _dataManagerQueueItem.EnqueueItem(_queueId, itemId, compressed, itemDescription, QueueItemStatus.New);
            }
            else
            {
                Dictionary<int, long> targetQueuesItemsIds = new Dictionary<int, long>();
                targetQueuesItemsIds.Add(_queueId, itemId);
                if (_subscribedQueueIds != null)
                {
                    foreach (int queueId in _subscribedQueueIds)
                        targetQueuesItemsIds.Add(queueId, _dataManagerQueueItem.GenerateItemID(queueId));
                }
                _dataManagerQueueItem.EnqueueItem(targetQueuesItemsIds, _queueId, itemId, compressed, itemDescription, QueueItemStatus.New);
            }
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
                try
                {
                    onItemReady(deserialized);
                    _dataManagerQueueItem.DeleteItem(_queueId, queueItem.ItemId);
                    _dataManagerQueueItem.UpdateHeaderStatus(queueItem.ItemId, QueueItemStatus.Processed);
                }
                catch(Exception ex)
                {
                    QueueItemHeader itemHeader = _dataManagerQueueItem.GetHeader(queueItem.ItemId, _queueId);
                    _dataManagerQueueItem.UpdateHeader(queueItem.ItemId, QueueItemStatus.Failed, itemHeader.RetryCount + 1, ex.ToString());
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

        DateTime _subscribedQueueIdsUpdatedTime;
        object _subscriptionsMaxTimeStamp;

        private void UpdateSubscribedQueueIdsIfNeeded()
        {
            if ((DateTime.Now - _subscribedQueueIdsUpdatedTime).TotalSeconds > 5)
            {
                lock (this)
                {
                    if ((DateTime.Now - _subscribedQueueIdsUpdatedTime).TotalSeconds > 5)
                    {
                        if (_dataManagerQueue.HaveSubscriptionsChanged(_subscriptionsMaxTimeStamp))
                        {
                            _subscribedQueueIds = new List<int>();
                            var subscriptions = _dataManagerQueue.GetSubscriptions();
                            if (subscriptions != null && subscriptions.Count > 0)
                            {
                                FillSubscribedQueueIdsFromSubscriptions(_queueId, subscriptions);
                            }
                            _subscriptionsMaxTimeStamp = _dataManagerQueue.GetSubscriptionsMaxTimestamp();
                        }
                        _subscribedQueueIdsUpdatedTime = DateTime.Now;
                    }
                }
            }
        }

        void FillSubscribedQueueIdsFromSubscriptions(int sourceQueueId, List<QueueSubscription> allSubscriptions)
        {
            foreach (var subscription in allSubscriptions.Where(itm => itm.QueueID == sourceQueueId))
            {
                if (subscription.SubsribedQueueID != _queueId && !_subscribedQueueIds.Contains(subscription.SubsribedQueueID))
                {
                    _subscribedQueueIds.Add(subscription.SubsribedQueueID);
                    FillSubscribedQueueIdsFromSubscriptions(subscription.SubsribedQueueID, allSubscriptions);
                }
            }
        }


        #endregion

        #region IPersistentQueue

        public void EnqueueObject(object item)
        {
            T typedItem = item as T;
            if (typedItem == null)
                throw new Exception(String.Format("item is not of type '{0}'", typeof(T)));
            this.Enqueue(typedItem);
        }

        #endregion
    }
}
