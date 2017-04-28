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
        QueueItemManager _queueItemManager = new QueueItemManager();

        QueueInstanceManager _queueManager = new QueueInstanceManager();
        QueueExecutionFlowManager _execFlowManager = new QueueExecutionFlowManager();
        ISummaryBatchActivatorDataManager _summaryBatchActivatorDataManager = QDataManagerFactory.GetDataManager<ISummaryBatchActivatorDataManager>();

        protected override void Execute()
        {
            TransactionLocker.Instance.TryLock("QueueRegulatorRuntimeService_Execute", () =>
            {
                List<QueueActivatorInstance> activeActivatorInstances = GetActiveActivatorInstances();
                AssignQueueItemsToActivators(activeActivatorInstances.Where(itm => itm.ActivatorType == QueueActivatorType.Normal).ToList());
                AssignSummaryBatchesToActivators(activeActivatorInstances.Where(itm => itm.ActivatorType == QueueActivatorType.Summary).ToList());
                TryUpdateHoldRequests();
            });
        }

        private void TryUpdateHoldRequests()
        {
            HoldRequestManager holdRequestManager = new HoldRequestManager();
            Dictionary<Guid, IOrderedEnumerable<HoldRequest>> holdRequestsByExecutionFlowDefinition = holdRequestManager.GetCachedOrderedHoldRequestsByExecutionFlowDefinition();
            if (holdRequestsByExecutionFlowDefinition == null || holdRequestsByExecutionFlowDefinition.Count == 0)
                return;

            foreach (var item in holdRequestsByExecutionFlowDefinition)
            {
                var holdRequests = item.Value;
                foreach (HoldRequest holdRequest in holdRequests)
                {
                    if (holdRequest.Status == HoldRequestStatus.CanBeStarted)
                        continue;

                    bool holdRequestCanBeStarted = true;

                    foreach (HoldRequest holdRequestToCompare in holdRequests)
                    {
                        if (holdRequest == holdRequestToCompare)
                            break;

                        if (holdRequest.IsOverlappedWith(holdRequestToCompare))
                        {
                            holdRequestCanBeStarted = false;
                            break;
                        }
                    }

                    if (!holdRequestCanBeStarted)
                        continue;

                    if (holdRequestCanBeStarted && holdRequest.QueuesToHold != null && holdRequest.QueuesToHold.Count > 0)
                    {
                        if (_queueItemManager.HasPendingQueueItems(holdRequest.QueuesToHold, holdRequest.From, holdRequest.To, true))
                            continue;

                        if (_queueItemManager.HasPendingSummaryQueueItems(holdRequest.QueuesToHold, holdRequest.From, holdRequest.To))
                            continue;
                    }

                    if (holdRequest.QueuesToProcess != null && holdRequest.QueuesToProcess.Count > 0)
                    {
                        if (_queueItemManager.HasPendingQueueItems(holdRequest.QueuesToProcess, holdRequest.From, holdRequest.To, false))
                            continue;

                        if (_summaryBatchActivatorDataManager.HasPendingSummaryBatchActivators(holdRequest.QueuesToProcess, holdRequest.From, holdRequest.To))
                            continue;
                    }

                    holdRequestManager.UpdateStatus(holdRequest.HoldRequestId, HoldRequestStatus.CanBeStarted);
                }
            }
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

            HoldRequestManager holdRequestManager = new HoldRequestManager();
            IOrderedEnumerable<HoldRequest> holdRequests = holdRequestManager.GetCachedOrderedHoldRequests();

            int maxNbOfPendingItems = s_maxNumberOfItemsPerActivator * activeActivatorInstances.Count;
            List<PendingQueueItemInfo> pendingQueueItems = _queueItemManager.GetPendingQueueItems(maxNbOfPendingItems, holdRequests);

            if (pendingQueueItems != null && pendingQueueItems.Count > 0)
            {
                Dictionary<int, AssignedQueueItemsCount> itemsCountByQueue;
                Dictionary<Guid, ActivatorInstanceItemsCount> itemsCountByActivator;
                BuildItemsCountObjs(activeActivatorInstances, pendingQueueItems, out itemsCountByQueue, out itemsCountByActivator);

                Dictionary<int, QueueRuntimeInfo> queueRuntimeInfos = _execFlowManager.GetQueueRuntimeInfoByQueueId();
                HashSet<int> sequentialQueueIds = new HashSet<int>(queueRuntimeInfos.Where(itm => itm.Value.IsSequencial).Select(itm => itm.Key));

                List<PendingQueueItemInfo> pendingQueueItemsToUpdate = new List<PendingQueueItemInfo>();
                List<ActivatorInstanceItemsCount> activatorsToAssign = itemsCountByActivator.Values.Where(itm => itm.ItemsCount < s_maxNumberOfItemsPerActivator).OrderBy(itm => itm.ItemsCount).ToList();
                int activatorInstanceIndex = 0;
                //assigning items of non-sequential queues
                foreach (var pendingQueueItem in pendingQueueItems.Where(itm => !itm.ActivatorInstanceId.HasValue && !sequentialQueueIds.Contains(itm.QueueId)).OrderBy(itm => itm.ExecutionFlowTriggerItemID).ThenBy(itm => itm.QueueItemId))
                {
                    if (activatorsToAssign.Count == 0)
                        break;

                    AssignedQueueItemsCount assignedQueueItemsCount = itemsCountByQueue[pendingQueueItem.QueueId];
                    if (assignedQueueItemsCount.ItemsCount >= assignedQueueItemsCount.MaxItemsCount)
                        continue;
                
                    AssignActivatorToQueueItem(pendingQueueItem, pendingQueueItemsToUpdate, assignedQueueItemsCount, ref activatorInstanceIndex, activatorsToAssign);
                }
                //assigning items of sequential queues
                foreach(var pendingQueueItem in pendingQueueItems.Where(itm => !itm.ActivatorInstanceId.HasValue && sequentialQueueIds.Contains(itm.QueueId)).OrderBy(itm => itm.QueueItemId))
                {
                    if (activatorsToAssign.Count == 0)
                        break;

                    AssignedQueueItemsCount assignedQueueItemsCount = itemsCountByQueue[pendingQueueItem.QueueId];
                    if (assignedQueueItemsCount.ItemsCount >= assignedQueueItemsCount.MaxItemsCount)
                        continue;

                    QueueRuntimeInfo queueRuntimeInfo = queueRuntimeInfos.GetRecord(pendingQueueItem.QueueId);
                    queueRuntimeInfo.ThrowIfNull("queueRuntimeInfo", pendingQueueItem.QueueId);
                    ActivatorInstanceItemsCount activatorToAssign = null;

                    var assignedQueueItemInSameStage = pendingQueueItems.FindRecord(itm => itm.ActivatorInstanceId.HasValue && queueRuntimeInfo.QueueIdsInSameStage.Contains(itm.QueueId));
                    if (assignedQueueItemInSameStage != null)
                    {                        
                        activatorToAssign = activatorsToAssign.FindRecord(itm => itm.ActivatorInstance.ActivatorId == assignedQueueItemInSameStage.ActivatorInstanceId.Value);
                        if (activatorToAssign == null)
                            continue;
                    }
                    AssignActivatorToQueueItem(pendingQueueItem, pendingQueueItemsToUpdate, assignedQueueItemsCount, ref activatorInstanceIndex, activatorsToAssign, activatorToAssign);
                }

                NotifyActivators(itemsCountByActivator, pendingQueueItemsToUpdate);
            }
        }

        private void BuildItemsCountObjs(List<QueueActivatorInstance> activeActivatorInstances, List<PendingQueueItemInfo> pendingQueueItems, out Dictionary<int, AssignedQueueItemsCount> itemsCountByQueue, out Dictionary<Guid, ActivatorInstanceItemsCount> itemsCountByActivator)
        {
            itemsCountByQueue = new Dictionary<int, AssignedQueueItemsCount>();
            itemsCountByActivator = activeActivatorInstances.ToDictionary(activatorInstance => activatorInstance.ActivatorId,
                activatorInstance => new ActivatorInstanceItemsCount
                {
                    ActivatorInstance = activatorInstance,
                    QueueIds = new List<int>()
                });
            foreach (var pendingQueueItem in pendingQueueItems)
            {
                AssignedQueueItemsCount assignedQueueItemsCount = itemsCountByQueue.GetOrCreateItem(pendingQueueItem.QueueId, () =>
                {
                    var queueInstance = _queueManager.GetQueueInstanceById(pendingQueueItem.QueueId);
                    return new AssignedQueueItemsCount { QueueId = pendingQueueItem.QueueId, MaxItemsCount = queueInstance.Settings.Activator.NbOfMaxConcurrentActivators };
                });
                if (pendingQueueItem.ActivatorInstanceId.HasValue)
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
        }

        private void AssignActivatorToQueueItem(PendingQueueItemInfo pendingQueueItem, List<PendingQueueItemInfo> pendingQueueItemsToUpdate, AssignedQueueItemsCount assignedQueueItemsCount, ref int activatorInstanceIndex, List<ActivatorInstanceItemsCount> activatorsToAssign, ActivatorInstanceItemsCount activatorToAssign = null)
        {
            if (activatorToAssign == null)
            {
                if (activatorInstanceIndex >= activatorsToAssign.Count)
                    activatorInstanceIndex = 0;

                activatorToAssign = activatorsToAssign[activatorInstanceIndex];
            }

            pendingQueueItem.ActivatorInstanceId = activatorToAssign.ActivatorInstance.ActivatorId;
            pendingQueueItemsToUpdate.Add(pendingQueueItem);

            assignedQueueItemsCount.ItemsCount++;
            activatorToAssign.QueueIds.Add(pendingQueueItem.QueueId);
            activatorToAssign.ItemsCount++;

            if (activatorToAssign.ItemsCount >= s_maxNumberOfItemsPerActivator)
                activatorsToAssign.Remove(activatorToAssign);

            activatorInstanceIndex++;
        }

        private void NotifyActivators(Dictionary<Guid, ActivatorInstanceItemsCount> itemsCountByActivator, List<PendingQueueItemInfo> pendingQueueItemsToUpdate)
        {
            if (pendingQueueItemsToUpdate != null && pendingQueueItemsToUpdate.Count > 0)
                _queueItemDataManager.SetQueueItemsActivatorInstances(pendingQueueItemsToUpdate);
            Parallel.ForEach(itemsCountByActivator.Values.Where(itm => itm.ItemsCount > 0),
                (activatorInstanceItemsCount) =>
                {
                    try
                    {
                        ServiceClientFactory.CreateTCPServiceClient<IQueueActivationRuntimeWCFService>(activatorInstanceItemsCount.ActivatorInstance.ServiceURL,
                            (client) =>
                            {
                                client.SetPendingQueuesToProcess(activatorInstanceItemsCount.ActivatorInstance.ActivatorId, activatorInstanceItemsCount.QueueIds.Distinct().ToList());
                            });
                    }
                    catch (Exception ex)
                    {
                        LoggerFactory.GetExceptionLogger().WriteException(ex);
                    }
                });
        }

        private void AssignSummaryBatchesToActivators(List<QueueActivatorInstance> activeActivatorInstances)
        {
            if (activeActivatorInstances == null || activeActivatorInstances.Count == 0)
                return;

            HoldRequestManager holdRequestManager = new HoldRequestManager();
            IOrderedEnumerable<HoldRequest> holdRequests = holdRequestManager.GetCachedOrderedHoldRequests();

            List<SummaryBatch> summaryBatches = _queueItemManager.GetSummaryBatches(holdRequests);
            if (summaryBatches != null && summaryBatches.Count > 0)
            {
                List<SummaryBatchActivator> assignedSummaryBatchActivators = _summaryBatchActivatorDataManager.GetAllSummaryBatchActivators();

                Dictionary<int, AssignedQueueItemsCount> assignedBatchesCountByQueue = new Dictionary<int, AssignedQueueItemsCount>();
                Dictionary<Guid, ActivatorInstanceItemsCount> itemsCountByActivator
                    = activeActivatorInstances.ToDictionary(activatorInstance => activatorInstance.ActivatorId, activatorInstance => new ActivatorInstanceItemsCount { ActivatorInstance = activatorInstance });

                for (int i = assignedSummaryBatchActivators.Count - 1; i >= 0; i--)
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

                foreach (var summarybatch in summaryBatches.OrderBy(itm => itm.BatchStart))
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
                        ActivatorId = activatorToAssign.ActivatorInstance.ActivatorId,
                        BatchEnd = summarybatch.BatchEnd
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
            public int QueueId { get; set; }

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
