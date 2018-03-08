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
using Vanrise.Runtime.Entities;

namespace Vanrise.Queueing
{
    public class QueueActivationService : RuntimeService
    {
        //QueueInstanceManager _queueInstanceManager = new QueueInstanceManager();
        //ConcurrentDictionary<string, object> _nonEmptyQueueNames = new ConcurrentDictionary<string, object>(); 
        ////Task _taskKeepDequeueing;
        ////Task _taskCheckNonEmptyQueues;
        //static int s_consecutiveItemsToProcess;
        //static QueueActivationService()
        //{
        //    if (!int.TryParse(ConfigurationManager.AppSettings["Queue_ConsecutiveItemsToProcess"], out s_consecutiveItemsToProcess))
        //        s_consecutiveItemsToProcess = 5;
        //}

        //Task _taskProcessing;

        public override void Execute()
        {
            //if (_taskProcessing == null)
            //    CreateTaskProcessing();            
        }

        //private void CreateTaskProcessing()
        //{
        //    _taskProcessing = new Task(() =>
        //    {
        //        GC.Collect();
        //        try
        //        {
        //            IEnumerable<QueueInstance> allQueues = _queueInstanceManager.GetReadyQueueInstances().Where(queue => queue.Settings.Activator is ISummaryBatchQueueActivator);
        //            foreach (var queueInstance in allQueues)
        //            {
        //                if (queueInstance.Settings.Activator == null)
        //                    continue;
        //                Vanrise.Runtime.TransactionLocker.Instance.TryLock(String.Format("QueueActivatorDequeue_Queue_{0}", queueInstance.QueueInstanceId),
        //                   queueInstance.Settings.Activator.NbOfMaxConcurrentActivators,
        //                             () =>
        //                             {
        //                                 int processedItems = 0;
        //                                 while (processedItems < s_consecutiveItemsToProcess && TryDequeueOneItem(queueInstance))
        //                                 {
        //                                     processedItems++;
        //                                 }
        //                             });
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            LoggerFactory.GetExceptionLogger().WriteException(ex);
        //        }
        //        finally
        //        {
        //            _taskProcessing = null;
        //        }
        //    });
        //    _taskProcessing.Start();
        //}

        ////private void CreateTaskCheckNonEmptyQueues()
        ////{
        ////    _taskCheckNonEmptyQueues = new Task(() =>
        ////    {
        ////        GC.Collect();
        ////        try
        ////        {
        ////            IEnumerable<QueueInstance> allQueues = _queueInstanceManager.GetReadyQueueInstances();
        ////            foreach (var queueInstance in allQueues)
        ////            {
        ////                if (!_nonEmptyQueueNames.ContainsKey(queueInstance.Name))
        ////                {
        ////                    if (TryDequeueOneItem(queueInstance))
        ////                    {
        ////                        _nonEmptyQueueNames.TryAdd(queueInstance.Name, null);
        ////                    }
        ////                }
        ////            }
        ////        }
        ////        catch (Exception ex)
        ////        {
        ////            LoggerFactory.GetExceptionLogger().WriteException(ex);
        ////        }
        ////        finally
        ////        {
        ////            _taskCheckNonEmptyQueues = null;
        ////        }
        ////    });
        ////    _taskCheckNonEmptyQueues.Start();
             
        ////}

        ////private void CreateTaskKeepDequeueing()
        ////{
        ////    _taskKeepDequeueing = new Task(() =>
        ////    {
        ////        try
        ////        {
        ////            while (_nonEmptyQueueNames.Count > 0)
        ////            {
        ////                string[] queueNames = _nonEmptyQueueNames.Keys.ToArray();
        ////                foreach (var queueName in queueNames)
        ////                {
        ////                    var queueInstance = _queueInstanceManager.GetQueueInstance(queueName);
        ////                    if (!TryDequeueOneItem(queueInstance))
        ////                    {
        ////                        Object dummy;
        ////                        _nonEmptyQueueNames.TryRemove(queueName, out dummy);
        ////                    }
        ////                }
        ////            }
        ////        }
        ////        catch (Exception ex)
        ////        {
        ////            LoggerFactory.GetExceptionLogger().WriteException(ex);
        ////        }
        ////        finally
        ////        {
        ////            _taskKeepDequeueing = null;
        ////        }
        ////    });
        ////    _taskKeepDequeueing.Start();
        ////}

        //private bool TryDequeueOneItem(QueueInstance queueInstance)
        //{
        //    try
        //    {
        //        var queue = PersistentQueueFactory.Default.GetQueue(queueInstance.Name);
        //        if (queue.QueueSettings.Activator == null)
        //            return false;
        //        ISummaryBatchQueueActivator summaryBatchQueueActivator = queue.QueueSettings.Activator as ISummaryBatchQueueActivator;
        //        if (summaryBatchQueueActivator != null)
        //            return TryDequeueFromSummaryBatchQueue(queue, summaryBatchQueueActivator);
        //        else
        //        {
        //            //int? nbOfMaxConcurrentQueueActivators = GetNbOfMaxConcurrentQueueActivators(queue.QueueSettings.Activator);

        //            Func<bool> dequeueAction = () =>
        //                {
        //                    if (queue.TryDequeueObject((itemToProcess) =>
        //                        {
        //                            QueueActivatorExecutionContext context = new QueueActivatorExecutionContext(itemToProcess, queueInstance);
        //                            queueInstance.Settings.Activator.ProcessItem(context);

        //                            if (context.OutputItems != null && context.OutputItems.Count > 0)
        //                            {
        //                                if (queueInstance.ExecutionFlowId != null)
        //                                {
        //                                    QueueExecutionFlowManager executionFlowManager = new QueueExecutionFlowManager();
        //                                    var queuesByStages = executionFlowManager.GetQueuesByStages(queueInstance.ExecutionFlowId.Value);
        //                                    foreach (var outputItem in context.OutputItems)
        //                                    {
        //                                        outputItem.Item.ExecutionFlowTriggerItemId = itemToProcess.ExecutionFlowTriggerItemId;
        //                                        var outputQueue = queuesByStages[outputItem.StageName].Queue;
        //                                        outputQueue.EnqueueObject(outputItem.Item);
        //                                    }
        //                                }
        //                                else
        //                                    throw new NullReferenceException(String.Format("queueInstance.ExecutionFlowId. Queue Instance ID {0}", queueInstance.QueueInstanceId));
        //                            }
        //                        }, null))
        //                    {
        //                        return true;
        //                    }
        //                    else
        //                        return false;

        //                };
        //            bool hasItem = false;
        //            //if(nbOfMaxConcurrentQueueActivators.HasValue)
        //            //{
        //            //    Vanrise.Runtime.TransactionLocker.Instance.TryLock(String.Format("QueueActivatorDequeue_Queue_{0}", queueInstance.QueueInstanceId), 
        //            //        nbOfMaxConcurrentQueueActivators.Value
        //            //        , () => hasItem = dequeueAction());
        //            //}
        //            //else
        //            //{
        //            hasItem = dequeueAction();
        //            //}
        //            return hasItem;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LoggerFactory.GetExceptionLogger().WriteException(ex);
        //    }
        //    return false;
        //}

        ////ConcurrentDictionary<int, int?> _maxConcurrenctQueueActivators = new ConcurrentDictionary<int, int?>();
        //private int? GetNbOfMaxConcurrentQueueActivators(QueueActivator queueActivator)
        //{
        //    return queueActivator.NbOfMaxConcurrentActivators;
        //    //int? nbOfMaxActivators;
        //    //if(!_maxConcurrenctQueueActivators.TryGetValue(queueActivator.ConfigId, out nbOfMaxActivators))
        //    //{
        //    //    int nbOfMaxActivators_Local;
        //    //    if (int.TryParse(ConfigurationManager.AppSettings[String.Format("Queue_NbOfMaxConcurrentActivators_{0}", queueActivator.ConfigId)], out nbOfMaxActivators_Local))
        //    //        nbOfMaxActivators = nbOfMaxActivators_Local;
        //    //    else
        //    //        nbOfMaxActivators = null;
        //    //} 
        //    //return nbOfMaxActivators;
        //}

        //private bool TryDequeueFromSummaryBatchQueue(IPersistentQueue batchPersistentQueue, ISummaryBatchQueueActivator summaryBatchQueueActivator)
        //{
        //    var batchStarts = batchPersistentQueue.GetAvailableBatchStarts();
        //    foreach (var batchStart in batchStarts)
        //    {
        //        bool isLocked = false;
        //        //unlocking the batch if it faces any problem while trying to lock it
        //        try
        //        {
        //            isLocked = summaryBatchQueueActivator.TryLock(batchStart);
        //        }
        //        catch
        //        {
        //            try
        //            {
        //                summaryBatchQueueActivator.Unlock(batchStart);
        //            }
        //            catch
        //            {

        //            }
        //            throw;
        //        }
        //        if (isLocked)
        //        {
        //            bool batchesUpdated;
        //            bool anyItemUpdated = false;
        //            try
        //            {
        //                do
        //                {
        //                    batchesUpdated = batchPersistentQueue.TryDequeueSummaryBatches(batchStart, (newBatches) =>
        //                    {
        //                        summaryBatchQueueActivator.UpdateNewBatches(batchStart, newBatches);
        //                    });
        //                    if (batchesUpdated)
        //                        anyItemUpdated = true;
        //                }
        //                while (batchesUpdated);
        //            }
        //            finally
        //            {
        //                summaryBatchQueueActivator.Unlock(batchStart);
        //            }
        //            return anyItemUpdated;
        //        }
        //    }

        //    return false;
        //}
    }
}
