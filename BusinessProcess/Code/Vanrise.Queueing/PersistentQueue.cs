using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Vanrise.Queueing
{
    public class PersistentQueue<T> : BaseQueue<T> where T : PersistentQueueItem
    {
        #region static

        static System.Timers.Timer s_TimerUpdateProcessHeartBeat;
        static List<RunningProcessInfo> s_runningProcesses;

        static PersistentQueue()
        {
           
            s_TimerUpdateProcessHeartBeat = new System.Timers.Timer();
            s_TimerUpdateProcessHeartBeat.Elapsed += s_TimerUpdateProcessHeartBeat_Elapsed;
        }

        static bool s_isRunning;
        static void s_TimerUpdateProcessHeartBeat_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (s_isRunning)
                return;
            lock (s_lockObj) 
            s_isRunning = true;

            

            lock (s_lockObj)
                s_isRunning = false;
        }

        static object s_lockObj = new object();
        static int s_numberOfProcessingItems;
        static void IncrementProcessing(int queueId)
        {
            lock(s_lockObj)
            {
                s_numberOfProcessingItems++;
                if (!s_TimerUpdateProcessHeartBeat.Enabled)
                    s_TimerUpdateProcessHeartBeat.Enabled = true;
            }
        }

        static void DecrementProcessing(int queueId)
        {
            lock (s_lockObj)
            {
                s_numberOfProcessingItems--;
                if (s_numberOfProcessingItems == 0)
                    s_TimerUpdateProcessHeartBeat.Enabled = false;
            }
        }

        #endregion

        #region ctor/Fields

        IQueueDataManager _dataManager;
        IQueueItemHeaderDataManager _itemHeaderDataManager;
        RunningProcessManager _runningProcessManager;
        int _queueId;
        T _emptyObject;
        internal PersistentQueue(string queueName)
        {
            _dataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
            _itemHeaderDataManager = QDataManagerFactory.GetDataManager<IQueueItemHeaderDataManager>();
            _runningProcessManager = new RunningProcessManager();
            _emptyObject = Activator.CreateInstance<T>();
            _queueId = _dataManager.GetQueue(queueName);
        }

        #endregion

        #region Properties

        public bool IsListening { get; private set; }

        #endregion

        #region Public Methods

        public override void Enqueue(T item)
        {
            item.Description = item.GenerateDescription();
            byte[] serialized = item.Serialize();
            byte[] compressed = Vanrise.Common.Compressor.Compress(serialized);
            Guid itemId = Guid.NewGuid();
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                _dataManager.EnqueueItem(_queueId, itemId, compressed);
                _itemHeaderDataManager.Insert(itemId, _queueId, item.Description, QueueItemStatus.New);
                scope.Complete();
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

        public void StartListening(Action<T> processItem, int maxThreads = 1)
        {
            if (processItem == null)
                throw new ArgumentNullException("processItem");
            if (this.IsListening)
                throw new InvalidOperationException("Listening is already started on this queue");

            lock (this)
                this.IsListening = true;

            Task task = new Task(() =>
            {
                int consecutiveFoundItems = 0;
                do
                {
                    if (TryDequeuePrivate(processItem, false))//if item is returned from the queue
                        consecutiveFoundItems++;
                    else
                    {
                        if (consecutiveFoundItems > 0)
                            consecutiveFoundItems--;
                        Thread.Sleep(1000);
                    }

                    //if many items are found in the queue consecutively, initialize multiple concurrent threads to dequeue and process items
                    if (consecutiveFoundItems > 1 && maxThreads > 1)
                    {
                        Parallel.For(0, maxThreads, (i) =>
                        {
                            bool hasItem = false;
                            do
                            {
                                hasItem = TryDequeuePrivate(processItem, false);
                            }
                            while (this.IsListening && hasItem);
                        });
                    }
                }
                while (this.IsListening);
            });
            task.Start();
        }

        public void StopListening()
        {
            lock (this)
                this.IsListening = false;
        }

        #endregion

        #region Private Methods

        private bool TryDequeuePrivate(Action<T> onItemReady, bool rethrowOnError)
        {
            int currentProcessId = RunningProcessManager.CurrentProcess.ProcessId;
            IEnumerable<int> runningProcessesIds = _runningProcessManager.GetCachedRunningProcesses(new TimeSpan(0, 1, 0)).Select(itm => itm.ProcessId);
            QueueItem queueItem = _dataManager.DequeueItem(_queueId, currentProcessId, runningProcessesIds);
            if (queueItem != null)
            {
                _itemHeaderDataManager.UpdateStatus(queueItem.ItemId, QueueItemStatus.Processing);
                byte[] decompressed = Vanrise.Common.Compressor.Decompress(queueItem.Content);
                T deserialized = _emptyObject.Deserialize<T>(decompressed);
                try
                {
                    onItemReady(deserialized);
                    _dataManager.DeleteItem(_queueId, queueItem.ItemId);
                    _itemHeaderDataManager.UpdateStatus(queueItem.ItemId, QueueItemStatus.Processed);
                }
                catch(Exception ex)
                {
                    QueueItemHeader itemHeader = _itemHeaderDataManager.Get(queueItem.ItemId);
                    _itemHeaderDataManager.Update(queueItem.ItemId, QueueItemStatus.Failed, itemHeader.RetryCount + 1, ex.ToString());
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
    }
}
