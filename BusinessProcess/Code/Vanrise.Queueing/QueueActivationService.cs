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
            if(queueIdsWithNewItems != null && queueIdsWithNewItems.Count > 0)
            {
                IQueueDataManager dataManagerQueue = QDataManagerFactory.GetDataManager<IQueueDataManager>();
                List<QueueInstance> updatedQueues = dataManagerQueue.GetQueueInstances(queueIdsWithNewItems.Keys);
                foreach(var queueInstance in updatedQueues)
                {
                    if (queueInstance.Settings != null && queueInstance.Settings.QueueActivator != null)
                    {
                        Task task = new Task(() =>
                        {
                            try
                            {
                                using (var queueActivator = queueInstance.Settings.QueueActivator)
                                {
                                    queueActivator.Run(queueInstance);
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

        protected void Execute2()
        {
            IQueueItemDataManager dataManagerQueueItem = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
            Dictionary<int, long> queueIdsWithNewItems = dataManagerQueueItem.GetQueueIDsHavingNewItems(_lastRetrievedQueueActivationId);
            if (queueIdsWithNewItems != null && queueIdsWithNewItems.Count > 0)
            {
                IQueueDataManager dataManagerQueue = QDataManagerFactory.GetDataManager<IQueueDataManager>();
                List<QueueInstance> updatedQueues = dataManagerQueue.GetQueueInstances(queueIdsWithNewItems.Keys);
                foreach (var queueInstance in updatedQueues)
                {
                    if (queueInstance.Settings != null && queueInstance.Settings.QueueActivator != null)
                    {
                        Task task = new Task(() =>
                        {
                            try
                            {
                                using (var queueActivator = queueInstance.Settings.QueueActivator)
                                {
                                    var queue = PersistentQueueFactory.Default.GetQueue(queueInstance.Name);
                                    if(queueInstance.ExecutionFlowId != null)
                                    {
                                        QueueExecutionFlowManager executionFlowManager = new QueueExecutionFlowManager();
                                        var queuesByStages = executionFlowManager.GetQueuesByStages(queueInstance.ExecutionFlowId.Value);
                                        queueActivator.Run(queueInstance, queue, queuesByStages);
                                    }
                                    queueActivator.Run(queueInstance);
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
