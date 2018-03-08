using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Vanrise.Queueing
{
    public class QueueActivationRuntimeService : RuntimeService
    {
        internal const string SERVICE_TYPE_UNIQUE_NAME = "VR_Queueing_QueueActivationRuntimeService";

        public override string ServiceTypeUniqueName
        {
            get
            {
                return SERVICE_TYPE_UNIQUE_NAME;
            }
        }

        IQueueItemDataManager _queueItemDataManager = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
        QueueInstanceManager _queueManager = new QueueInstanceManager();
        QueueExecutionFlowManager _executionFlowManager = new QueueExecutionFlowManager();
        public override void Execute()
        {
            int queueIdToProcess;
            while (PendingItemsHandler.Current.TryGetPendingQueueToProcess(base.ServiceInstance.ServiceInstanceId, out queueIdToProcess))
            {
                QueueInstance queueInstance = _queueManager.GetQueueInstanceById(queueIdToProcess);
                if (queueInstance == null)
                    throw new NullReferenceException(String.Format("queueInstance. {0}", queueIdToProcess));
                IPersistentQueue queue = PersistentQueueFactory.Default.GetQueue(queueIdToProcess);
                var queuesByStages = _executionFlowManager.GetQueuesByStages(queueInstance.ExecutionFlowId.Value);
                var dequeueContext = new PersistentQueueDequeueContext { ActivatorInstanceId = base.ServiceInstance.ServiceInstanceId };
                bool hasItem = false;
                do
                {
                    Action<PersistentQueueItem, QueueItem> processItem =
                        (itemToProcess, queueItem) =>
                        {
                            QueueActivatorExecutionContext context = new QueueActivatorExecutionContext(itemToProcess, queueInstance, queueItem);
                            queueInstance.Settings.Activator.ProcessItem(context);

                            if (context.OutputItems != null && context.OutputItems.Count > 0)
                            {
                                if (queueInstance.ExecutionFlowId.HasValue)
                                {
                                    foreach (var outputItem in context.OutputItems)
                                    {
                                        outputItem.Item.ExecutionFlowTriggerItemId = itemToProcess.ExecutionFlowTriggerItemId;
                                        outputItem.Item.DataSourceID = itemToProcess.DataSourceID;
                                        outputItem.Item.BatchDescription = itemToProcess.BatchDescription;
                                        var outputQueue = queuesByStages[outputItem.StageName].Queue;
                                        outputQueue.EnqueueObject(outputItem.Item);
                                    }
                                }
                                else
                                    throw new NullReferenceException(String.Format("queueInstance.ExecutionFlowId. Queue Instance ID {0}", queueInstance.QueueInstanceId));
                            }
                        };
                    hasItem = queue.TryDequeueObject(processItem, dequeueContext);
                }
                while (hasItem);
            }
        }



    }
}
