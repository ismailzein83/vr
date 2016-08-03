using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;
using Vanrise.Common;

namespace Vanrise.Queueing
{
    public class QueueRegulatorRuntimeService : RuntimeService
    {
        static int s_maxNumberOfItemsPerActivator = 5;

        IQueueActivatorInstanceDataManager _queueActivatorInstanceDataManager = QDataManagerFactory.GetDataManager<IQueueActivatorInstanceDataManager>();
        IQueueItemDataManager _queueItemDataManager = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();

        QueueInstanceManager _queueManager = new QueueInstanceManager();
        ISummaryBatchActivatorDataManager _summaryBatchActivatorDataManager = QDataManagerFactory.GetDataManager<ISummaryBatchActivatorDataManager>();

        protected override void Execute()
        {
            TransactionLocker.Instance.TryLock("QueueRegulatorRuntimeService_Execute", () =>
            {
                List<QueueActivatorInstance> activeActivatorInstances = GetActiveActivatorInstances();
                AssignQueueItemsToActivators(activeActivatorInstances.Where(itm => itm.ActivatorType == QueueActivatorType.Normal).ToList());
                AssignSummaryBatchesToActivators(activeActivatorInstances.Where(itm => itm.ActivatorType == QueueActivatorType.Summary).ToList());
            });
        }

        private List<QueueActivatorInstance> GetActiveActivatorInstances()
        {
            List<QueueActivatorInstance> allActivatorInstances = _queueActivatorInstanceDataManager.GetAll();
            if (allActivatorInstances != null)
            {
                List<QueueActivatorInstance> activeActivatorInstances = new List<QueueActivatorInstance>();
                HashSet<int> runningProcessIds = new HashSet<int>(new RunningProcessManager().GetCachedRunningProcesses().Select(itm => itm.ProcessId));
                foreach (var activatorInstance in allActivatorInstances)
                {
                    if (runningProcessIds.Contains(activatorInstance.ProcessId))
                        activeActivatorInstances.Add(activatorInstance);
                    else
                        _queueActivatorInstanceDataManager.Delete(activatorInstance.ActivatorId);
                }
                return activeActivatorInstances;
            }
            else
                return null;
        }
                
        private void AssignQueueItemsToActivators(List<QueueActivatorInstance> activeActivatorInstances)
        {
            if (activeActivatorInstances == null || activeActivatorInstances.Count == 0)
                return;
            List<PendingQueueItemInfo> pendingQueueItems = _queueItemDataManager.GetPendingQueueItems();
            if(pendingQueueItems != null && pendingQueueItems.Count > 0)
            {
                Dictionary<int, AssignedQueueItemsCount> itemsCountByQueue = new Dictionary<int, AssignedQueueItemsCount>();
                Dictionary<Guid, ActivatorInstanceItemsCount> itemsCountByActivator
                    = activeActivatorInstances.ToDictionary(activatorInstance => activatorInstance.ActivatorId, activatorInstance => new ActivatorInstanceItemsCount
                    {
                        ActivatorInstance = activatorInstance,
                        QueueIds = new List<int>()
                    });
                foreach(var pendingQueueItem in pendingQueueItems)
                {
                    AssignedQueueItemsCount assignedQueueItemsCount = itemsCountByQueue.GetOrCreateItem(pendingQueueItem.QueueId, () =>
                        {
                            var queueInstance = _queueManager.GetQueueInstanceById(pendingQueueItem.QueueId);
                            return new AssignedQueueItemsCount { QueueId = pendingQueueItem.QueueId, MaxItemsCount = queueInstance.Settings.Activator.NbOfMaxConcurrentActivators };
                        });
                    if(pendingQueueItem.ActivatorInstanceId.HasValue)
                    {
                        ActivatorInstanceItemsCount activatorItemsCount;
                        if (itemsCountByActivator.TryGetValue(pendingQueueItem.ActivatorInstanceId.Value, out activatorItemsCount))
                        {
                            activatorItemsCount.ItemsCount++;
                            activatorItemsCount.QueueIds.Add(pendingQueueItem.QueueId);
                            assignedQueueItemsCount.ItemsCount++;
                        }
                        else
                            pendingQueueItem.ActivatorInstanceId = null;
                    }
                }

                List<PendingQueueItemInfo> pendingQueueItemsToUpdate = new List<PendingQueueItemInfo>();
                List<ActivatorInstanceItemsCount> activatorsToAssign = itemsCountByActivator.Values.Where(itm => itm.ItemsCount < s_maxNumberOfItemsPerActivator).OrderBy(itm => itm.ItemsCount).ToList();
                int activatorInstanceIndex = 0;
                foreach(var pendingQueueItem in pendingQueueItems.Where(itm => !itm.ActivatorInstanceId.HasValue).OrderBy(itm => itm.QueueItemId))
                {
                    if (activatorsToAssign.Count == 0)
                        break;

                    AssignedQueueItemsCount assignedQueueItemsCount = itemsCountByQueue[pendingQueueItem.QueueId];
                    if (assignedQueueItemsCount.ItemsCount >= assignedQueueItemsCount.MaxItemsCount)
                        continue;

                    if (activatorInstanceIndex >= activatorsToAssign.Count)
                        activatorInstanceIndex = 0;

                    ActivatorInstanceItemsCount activatorToAssign = activatorsToAssign[activatorInstanceIndex];
                    pendingQueueItem.ActivatorInstanceId = activatorToAssign.ActivatorInstance.ActivatorId;
                    pendingQueueItemsToUpdate.Add(pendingQueueItem);

                    assignedQueueItemsCount.ItemsCount++;
                    activatorToAssign.QueueIds.Add(pendingQueueItem.QueueId);
                    activatorToAssign.ItemsCount++;

                    if (activatorToAssign.ItemsCount >= s_maxNumberOfItemsPerActivator)
                        activatorsToAssign.Remove(activatorToAssign);

                    activatorInstanceIndex++;
                }
                _queueItemDataManager.SetQueueItemsActivatorInstances(pendingQueueItemsToUpdate);
                foreach(var activatorInstanceItemsCount in itemsCountByActivator.Values)
                {
                    if(activatorInstanceItemsCount.ItemsCount > 0)
                    {
                        try
                        {
                            ServiceClientFactory.CreateTCPServiceClient<IQueueActivationRuntimeWCFService>(activatorInstanceItemsCount.ActivatorInstance.ServiceURL,
                                (client) =>
                                {
                                    client.SetPendingQueuesToProcess(activatorInstanceItemsCount.ActivatorInstance.ActivatorId, activatorInstanceItemsCount.QueueIds.Distinct().ToList());
                                });
                        }
                        catch(Exception ex)
                        {
                            LoggerFactory.GetExceptionLogger().WriteException(ex);
                        }
                    }
                }                
            }
        }

        private void AssignSummaryBatchesToActivators(List<QueueActivatorInstance> activeActivatorInstances)
        {
            if (activeActivatorInstances == null || activeActivatorInstances.Count == 0)
                return;
            List<SummaryBatch> summaryBatches = _queueItemDataManager.GetSummaryBatches();
            if(summaryBatches != null && summaryBatches.Count > 0)
            {
                List<SummaryBatchActivator> assignedSummaryBatchActivators = _summaryBatchActivatorDataManager.GetAllSummaryBatchActivators();
                
                Dictionary<int, AssignedQueueItemsCount> assignedBatchesCountByQueue = new Dictionary<int, AssignedQueueItemsCount>();
                Dictionary<Guid, ActivatorInstanceItemsCount> itemsCountByActivator
                    = activeActivatorInstances.ToDictionary(activatorInstance => activatorInstance.ActivatorId, activatorInstance => new ActivatorInstanceItemsCount { ActivatorInstance = activatorInstance });

                for(int i = assignedSummaryBatchActivators.Count -1; i>=0;i--)
                {
                    var summaryBatchActivator = assignedSummaryBatchActivators[i];
                    AssignedQueueItemsCount assignedQueueItemsCount = assignedBatchesCountByQueue.GetOrCreateItem(summaryBatchActivator.QueueId, () =>
                    {
                        var queueInstance = _queueManager.GetQueueInstanceById(summaryBatchActivator.QueueId);
                        return new AssignedQueueItemsCount { QueueId = summaryBatchActivator.QueueId, MaxItemsCount = queueInstance.Settings.Activator.NbOfMaxConcurrentActivators };
                    });
                    ActivatorInstanceItemsCount activatorItemsCount;
                    if (itemsCountByActivator.TryGetValue(summaryBatchActivator.ActivatorId, out activatorItemsCount))
                    {
                        activatorItemsCount.ItemsCount++;
                        assignedQueueItemsCount.ItemsCount++;
                    }
                    else
                    {
                        _summaryBatchActivatorDataManager.Delete(summaryBatchActivator.QueueId, summaryBatchActivator.BatchStart);
                        assignedSummaryBatchActivators.Remove(summaryBatchActivator);
                    }
                }

                List<SummaryBatchActivator> summaryBatchActivatorsToCreate = new List<SummaryBatchActivator>();
                List<ActivatorInstanceItemsCount> activatorsToAssign = itemsCountByActivator.Values.Where(itm => itm.ItemsCount < s_maxNumberOfItemsPerActivator).OrderBy(itm => itm.ItemsCount).ToList();
                int activatorInstanceIndex = 0;

                foreach(var summarybatch in summaryBatches.OrderBy(itm => itm.BatchStart))
                {
                    if (activatorsToAssign.Count == 0)
                        break;

                    if (assignedSummaryBatchActivators.Any(itm => itm.QueueId == summarybatch.QueueId && itm.BatchStart == summarybatch.BatchStart))
                    {
                        continue;
                    }

                    AssignedQueueItemsCount assignedQueueItemsCount = assignedBatchesCountByQueue.GetOrCreateItem(summarybatch.QueueId, () =>
                    {
                        var queueInstance = _queueManager.GetQueueInstanceById(summarybatch.QueueId);
                        return new AssignedQueueItemsCount { QueueId = summarybatch.QueueId, MaxItemsCount = queueInstance.Settings.Activator.NbOfMaxConcurrentActivators };
                    });
                    if (assignedQueueItemsCount.ItemsCount >= assignedQueueItemsCount.MaxItemsCount)
                        continue;

                    if (activatorInstanceIndex >= activatorsToAssign.Count)
                        activatorInstanceIndex = 0;

                    ActivatorInstanceItemsCount activatorToAssign = activatorsToAssign[activatorInstanceIndex];
                    summaryBatchActivatorsToCreate.Add(new SummaryBatchActivator
                    {
                        QueueId = summarybatch.QueueId,
                        BatchStart = summarybatch.BatchStart,
                        ActivatorId = activatorToAssign.ActivatorInstance.ActivatorId
                    });

                    assignedQueueItemsCount.ItemsCount++;
                    activatorToAssign.ItemsCount++;
                    if (activatorToAssign.ItemsCount >= s_maxNumberOfItemsPerActivator)
                        activatorsToAssign.Remove(activatorToAssign);

                    activatorInstanceIndex++;                    
                }
                _summaryBatchActivatorDataManager.Insert(summaryBatchActivatorsToCreate);
            }
        }


        #region Private Classes

        private class AssignedQueueItemsCount
        {
            public int QueueId{ get; set; }

            public int ItemsCount { get; set; }

            public int MaxItemsCount { get; set; }
        }

        private class ActivatorInstanceItemsCount
        {
            public QueueActivatorInstance ActivatorInstance { get; set; }

            public int ItemsCount { get; set; }

            public List<int> QueueIds { get; set; }
        }

        #endregion
    }
}
