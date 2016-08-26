using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;

namespace Vanrise.Queueing
{
    public class SummaryQueueActivationRuntimeService : RuntimeService
    {
        protected override void OnStarted(IRuntimeServiceStartContext context)
        {
            RegisterActivator();
            base.OnStarted(context);
        }

        Guid _activatorId;
        private void RegisterActivator()
        {
            _activatorId = Guid.NewGuid();
            var dataManager = QDataManagerFactory.GetDataManager<IQueueActivatorInstanceDataManager>();
            dataManager.InsertActivator(_activatorId, Vanrise.Runtime.RunningProcessManager.CurrentProcess.ProcessId, QueueActivatorType.Summary, null);
        }

        QueueInstanceManager _queueManager = new QueueInstanceManager();
        ISummaryBatchActivatorDataManager _summaryBatchActivatorDataManager = QDataManagerFactory.GetDataManager<ISummaryBatchActivatorDataManager>();

        protected override void Execute()
        {
            List<SummaryBatchActivator> summaryBatchActivators = _summaryBatchActivatorDataManager.GetSummaryBatchActivators(_activatorId);
            if(summaryBatchActivators != null && summaryBatchActivators.Count > 0)
            {
                foreach(var summaryBatchActivator in summaryBatchActivators)
                {
                    bool isLocked = false;
                    var queueInstance = _queueManager.GetQueueInstanceById(summaryBatchActivator.QueueId);
                    if (queueInstance == null)
                        throw new NullReferenceException(String.Format("queueInstance '{0}'", summaryBatchActivator.QueueId));
                    var summaryBatchQueueActivator = queueInstance.Settings.Activator as ISummaryBatchQueueActivator;
                    if(summaryBatchQueueActivator == null)
                        throw new NullReferenceException(String.Format("summaryBatchQueueActivator '{0}'", summaryBatchActivator.QueueId));
                    var batchPersistentQueue = PersistentQueueFactory.Default.GetQueue(summaryBatchActivator.QueueId);
                    //unlocking the batch if it faces any problem while trying to lock it
                    try
                    {
                        isLocked = summaryBatchQueueActivator.TryLock(summaryBatchActivator.BatchStart);
                    }
                    catch
                    {
                        try
                        {
                            summaryBatchQueueActivator.Unlock(summaryBatchActivator.BatchStart);
                        }
                        catch
                        {

                        }
                        throw;
                    }
                    if (isLocked)
                    {
                        bool batchesUpdated;
                        try
                        {
                            do
                            {
                                batchesUpdated = batchPersistentQueue.TryDequeueSummaryBatches(summaryBatchActivator.BatchStart, (newBatches) =>
                                {
                                    summaryBatchQueueActivator.UpdateNewBatches(summaryBatchActivator.BatchStart, newBatches);
                                });
                            }
                            while (batchesUpdated);
                            _summaryBatchActivatorDataManager.Delete(summaryBatchActivator.QueueId, summaryBatchActivator.BatchStart);
                        }
                        finally
                        {
                            summaryBatchQueueActivator.Unlock(summaryBatchActivator.BatchStart);
                        }
                    }
                }
            }
        }
    }
}
