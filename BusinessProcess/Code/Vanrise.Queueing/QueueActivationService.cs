using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities.SummaryTransformation;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;

namespace Vanrise.Queueing
{
    public class QueueActivationService : RuntimeService
    {
        QueueInstanceManager _queueInstanceManager = new QueueInstanceManager();
        ConcurrentDictionary<string, object> _nonEmptyQueueNames = new ConcurrentDictionary<string, object>(); 
        Task _taskKeepDequeueing;
        Task _taskCheckNonEmptyQueues;

        protected override void Execute()
        {            
            if (_taskCheckNonEmptyQueues == null)
                CreateTaskCheckNonEmptyQueues();
            if (_taskKeepDequeueing == null)
                CreateTaskKeepDequeueing();
        }

        private void CreateTaskCheckNonEmptyQueues()
        {
            _taskCheckNonEmptyQueues = new Task(() =>
            {
                try
                {
                    IEnumerable<QueueInstance> allQueues = _queueInstanceManager.GetReadyQueueInstances();
                    foreach (var queueInstance in allQueues)
                    {
                        if (!_nonEmptyQueueNames.ContainsKey(queueInstance.Name))
                        {
                            if (TryDequeueOneItem(queueInstance))
                            {
                                _nonEmptyQueueNames.TryAdd(queueInstance.Name, null);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerFactory.GetExceptionLogger().WriteException(ex);
                }
                finally
                {
                    _taskCheckNonEmptyQueues = null;
                }
            });
            _taskCheckNonEmptyQueues.Start();
             
        }

        private void CreateTaskKeepDequeueing()
        {
            _taskKeepDequeueing = new Task(() =>
            {
                try
                {
                    while (_nonEmptyQueueNames.Count > 0)
                    {
                        string[] queueNames = _nonEmptyQueueNames.Keys.ToArray();
                        foreach (var queueName in queueNames)
                        {
                            var queueInstance = _queueInstanceManager.GetQueueInstance(queueName);
                            if (!TryDequeueOneItem(queueInstance))
                            {
                                Object dummy;
                                _nonEmptyQueueNames.TryRemove(queueName, out dummy);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerFactory.GetExceptionLogger().WriteException(ex);
                }
                finally
                {
                    _taskKeepDequeueing = null;
                }
            });
            _taskKeepDequeueing.Start();
        }

        private bool TryDequeueOneItem(QueueInstance queueInstance)
        {
            try
            {
                var queue = PersistentQueueFactory.Default.GetQueue(queueInstance.Name);
                if (queueInstance.Settings.SummaryBatchManager != null)
                    return TryDequeueFromSummaryBatchQueue(queue, queueInstance);
                else
                {
                    if (queueInstance.ExecutionFlowId != null)
                    {
                        if (queue.TryDequeueObject((itemToProcess) =>
                        {
                            QueueActivatorExecutionContext context = new QueueActivatorExecutionContext(itemToProcess, queueInstance);
                            queueInstance.Settings.Activator.ProcessItem(context);
                            if (context.OutputItems != null && context.OutputItems.Count > 0)
                            {
                                QueueExecutionFlowManager executionFlowManager = new QueueExecutionFlowManager();
                                var queuesByStages = executionFlowManager.GetQueuesByStages(queueInstance.ExecutionFlowId.Value);
                                foreach (var outputItem in context.OutputItems)
                                {
                                    outputItem.Item.ExecutionFlowTriggerItemId = itemToProcess.ExecutionFlowTriggerItemId;
                                    var outputQueue = queuesByStages[outputItem.StageName].Queue;
                                    outputQueue.EnqueueObject(outputItem.Item);
                                }
                            }
                        }))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.GetExceptionLogger().WriteException(ex);
            }
            return false;
        }

        private bool TryDequeueFromSummaryBatchQueue(IPersistentQueue batchPersistentQueue, QueueInstance queueInstance)
        {
            var batchStarts = batchPersistentQueue.GetAvailableBatchStarts();
            foreach (var batchStart in batchStarts)
            {
                if (queueInstance.Settings.SummaryBatchManager.TryLock(batchStart))
                {
                    bool batchesUpdated;
                    bool anyItemUpdated = false;
                    try
                    {
                        do
                        {
                            batchesUpdated = batchPersistentQueue.TryDequeueSummaryBatches(batchStart, (newBatches) =>
                            {
                                queueInstance.Settings.SummaryBatchManager.UpdateNewBatches(batchStart, newBatches);
                            });
                            if (batchesUpdated)
                                anyItemUpdated = true;
                        }
                        while (batchesUpdated);
                    }
                    finally
                    {
                        queueInstance.Settings.SummaryBatchManager.Unlock(batchStart);
                    }
                    return anyItemUpdated;
                }
            }

            return false;
        }
    }
}
