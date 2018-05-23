using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Vanrise.Queueing
{
    public class SummaryQueueActivationRuntimeService : RuntimeService
    {

        public override Guid ConfigId { get { return new Guid("83028DF1-9BFE-494F-BD3F-9BE6ABE60833"); } }

        internal const string SERVICE_TYPE_UNIQUE_NAME = "VR_Queueing_SummaryQueueActivationRuntimeService";

        public override string ServiceTypeUniqueName
        {
            get
            {
                return SERVICE_TYPE_UNIQUE_NAME;
            }
        }

        QueueInstanceManager _queueManager = new QueueInstanceManager();
        ISummaryBatchActivatorDataManager _summaryBatchActivatorDataManager = QDataManagerFactory.GetDataManager<ISummaryBatchActivatorDataManager>();
        ExecutionControlManager _executionControlManager = new ExecutionControlManager();

        public override void Execute()
        {
            if (_executionControlManager.IsExecutionPaused())
                return;
            if (PendingItemsHandler.Current.HasSummaryItemsToProcess(base.ServiceInstance.ServiceInstanceId))
            {
                List<SummaryBatchActivator> summaryBatchActivators = _summaryBatchActivatorDataManager.GetSummaryBatchActivators(base.ServiceInstance.ServiceInstanceId);
                if (summaryBatchActivators != null && summaryBatchActivators.Count > 0)
                {
                    foreach (var summaryBatchActivator in summaryBatchActivators)
                    {
                        var queueInstance = _queueManager.GetQueueInstanceById(summaryBatchActivator.QueueId);
                        if (queueInstance == null)
                            throw new NullReferenceException(String.Format("queueInstance '{0}'", summaryBatchActivator.QueueId));
                        var summaryBatchQueueActivator = queueInstance.Settings.Activator as ISummaryBatchQueueActivator;
                        if (summaryBatchQueueActivator == null)
                            throw new NullReferenceException(String.Format("summaryBatchQueueActivator '{0}'", summaryBatchActivator.QueueId));
                        var batchPersistentQueue = PersistentQueueFactory.Default.GetQueue(summaryBatchActivator.QueueId);

                        bool batchesUpdated;
                        Object batchStartState = null;
                        do
                        {
                            batchesUpdated = batchPersistentQueue.TryDequeueSummaryBatches(summaryBatchActivator.BatchStart, (newBatches) =>
                            {
                                summaryBatchQueueActivator.UpdateNewBatches(summaryBatchActivator.BatchStart, newBatches, ref batchStartState);
                            });
                        }
                        while (batchesUpdated);
                        _summaryBatchActivatorDataManager.Delete(summaryBatchActivator.QueueId, summaryBatchActivator.BatchStart);
                    }
                }
            }
        }
    }
}