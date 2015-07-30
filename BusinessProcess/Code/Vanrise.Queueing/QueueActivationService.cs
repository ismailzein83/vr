using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;

namespace Vanrise.Queueing
{
    public class QueueActivationService : RuntimeService
    {
        long _lastRetrievedQueueActivationId;
        protected override void Execute()
        {
            IQueueItemDataManager dataManagerQueueItem = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
            Dictionary<int, long> queueIdsWithNewItems = dataManagerQueueItem.GetQueueIDsHavingNewItems(_lastRetrievedQueueActivationId);
            if (queueIdsWithNewItems != null && queueIdsWithNewItems.Count > 0)
            {
                IQueueDataManager dataManagerQueue = QDataManagerFactory.GetDataManager<IQueueDataManager>();
                List<QueueInstance> updatedQueues = dataManagerQueue.GetQueueInstances(queueIdsWithNewItems.Keys);
                foreach (var queueInstance in updatedQueues)
                {
                    if (queueInstance.Settings != null && !String.IsNullOrEmpty(queueInstance.Settings.QueueActivatorFQTN))
                    {
                        Task task = new Task(() =>
                        {
                            try
                            {
                                Type queueActivatorType = Type.GetType(queueInstance.Settings.QueueActivatorFQTN);
                                if (queueActivatorType == null)
                                    throw new Exception(String.Format("Could not load QueueActivator type '{0}'", queueInstance.Settings.QueueActivatorFQTN));
                                QueueActivator queueActivator = Activator.CreateInstance(queueActivatorType) as QueueActivator;
                                if(queueActivator == null)
                                    throw new Exception(String.Format("'{0}' is not of type Vanrise.Queueing.Entities.QueueActivator", queueInstance.Settings.QueueActivatorFQTN));
                                using (queueActivator)
                                {
                                    var queue = PersistentQueueFactory.Default.GetQueue(queueInstance.Name);
                                    if (queueInstance.ExecutionFlowId != null)
                                    {
                                        QueueExecutionFlowManager executionFlowManager = new QueueExecutionFlowManager();
                                        var queuesByStages = executionFlowManager.GetQueuesByStages(queueInstance.ExecutionFlowId.Value);
                                        while (queue.TryDequeueObject((itemToProcess) =>
                                        {
                                            ItemsToEnqueue outputItems = new ItemsToEnqueue();
                                            queueActivator.ProcessItem(itemToProcess, outputItems);
                                            if (outputItems.Count > 0)
                                            {
                                                foreach (var outputItem in outputItems)
                                                {
                                                    outputItem.Item.ExecutionFlowTriggerItemId = itemToProcess.ExecutionFlowTriggerItemId;
                                                    queuesByStages[outputItem.StageName].Queue.EnqueueObject(outputItem.Item);
                                                }
                                            }
                                        }))
                                        {

                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LoggerFactory.GetExceptionLogger().WriteException(ex);
                            }
                        });
                        task.Start();
                    }
                }
                _lastRetrievedQueueActivationId = queueIdsWithNewItems.Values.Max();
            }
        } 
    }
}
