using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;
using Vanrise.Common;
using Vanrise.Runtime.Entities;

namespace Vanrise.Queueing
{
    public class QueueRegulatorRuntimeService : RuntimeService
    {

        public override Guid ConfigId { get { return new Guid("B08F3D51-CC02-4292-9AAB-FB9A8720EE16"); } }

        static int s_maxNumberOfItemsPerService = 5;

        IQueueItemDataManager _queueItemDataManager = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
        QueueItemManager _queueItemManager = new QueueItemManager();

        QueueInstanceManager _queueManager = new QueueInstanceManager();
        QueueExecutionFlowManager _execFlowManager = new QueueExecutionFlowManager();
        ISummaryBatchActivatorDataManager _summaryBatchActivatorDataManager = QDataManagerFactory.GetDataManager<ISummaryBatchActivatorDataManager>();

        RuntimeServiceInstanceManager _serviceInstanceManager = new RuntimeServiceInstanceManager();

        public override void Execute()
        {
            TransactionLocker.Instance.TryLock("QueueRegulatorRuntimeService_Execute", () =>
            {
                List<Runtime.Entities.RuntimeServiceInstance> queueRuntimeServiceInstances = _serviceInstanceManager.GetServices(QueueActivationRuntimeService.SERVICE_TYPE_UNIQUE_NAME);
                AssignQueueItemsToServices(queueRuntimeServiceInstances);
                List<Runtime.Entities.RuntimeServiceInstance> summaryQueueRuntimeServiceInstances = _serviceInstanceManager.GetServices(SummaryQueueActivationRuntimeService.SERVICE_TYPE_UNIQUE_NAME);
                AssignSummaryBatchesToServices(summaryQueueRuntimeServiceInstances);
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

        private void AssignQueueItemsToServices(List<Runtime.Entities.RuntimeServiceInstance> activeServiceInstances)
        {
            if (activeServiceInstances == null || activeServiceInstances.Count == 0)
                return;

            HoldRequestManager holdRequestManager = new HoldRequestManager();
            IOrderedEnumerable<HoldRequest> holdRequests = holdRequestManager.GetCachedOrderedHoldRequests();

            int maxNbOfPendingItems = s_maxNumberOfItemsPerService * activeServiceInstances.Count;
            List<PendingQueueItemInfo> pendingQueueItems = _queueItemManager.GetPendingQueueItems(maxNbOfPendingItems, holdRequests);

            if (pendingQueueItems != null && pendingQueueItems.Count > 0)
            {
                Dictionary<int, AssignedQueueItemsCount> itemsCountByQueue;
                Dictionary<Guid, ServiceInstanceItemsCount> itemsCountByServiceId;
                HashSet<int> sequentialQueueIds;
                BuildItemsCountObjs(activeServiceInstances, pendingQueueItems, out itemsCountByQueue, out itemsCountByServiceId, out sequentialQueueIds);

                List<PendingQueueItemInfo> pendingQueueItemsToUpdate = new List<PendingQueueItemInfo>();
                List<ServiceInstanceItemsCount> servicesToAssign = itemsCountByServiceId.Values.Where(itm => itm.ItemsCount < s_maxNumberOfItemsPerService).OrderBy(itm => itm.ItemsCount).ToList();
                int serviceInstanceIndex = 0;
                //assigning items of non-sequential queues
                foreach (var pendingQueueItem in pendingQueueItems.Where(itm => !itm.ActivatorInstanceId.HasValue && !sequentialQueueIds.Contains(itm.QueueId)).OrderBy(itm => itm.ExecutionFlowTriggerItemID).ThenBy(itm => itm.QueueItemId))
                {
                    if (servicesToAssign.Count == 0)
                        break;

                    AssignedQueueItemsCount assignedQueueItemsCount = itemsCountByQueue[pendingQueueItem.QueueId];
                    if (assignedQueueItemsCount.ItemsCount >= assignedQueueItemsCount.MaxItemsCount)
                        continue;

                    AssignServiceToQueueItem(pendingQueueItem, pendingQueueItemsToUpdate, assignedQueueItemsCount, ref serviceInstanceIndex, servicesToAssign);
                }
                //assigning items of sequential queues
                foreach (var pendingQueueItem in pendingQueueItems.Where(itm => !itm.ActivatorInstanceId.HasValue && sequentialQueueIds.Contains(itm.QueueId)).OrderBy(itm => itm.QueueItemId))
                {
                    if (servicesToAssign.Count == 0)
                        break;

                    AssignedQueueItemsCount assignedQueueItemsCount = itemsCountByQueue[pendingQueueItem.QueueId];
                    if (assignedQueueItemsCount.ItemsCount >= assignedQueueItemsCount.MaxItemsCount)
                        continue;

                    ServiceInstanceItemsCount serviceToAssign = null;

                    var assignedQueueItemInSameStage = pendingQueueItems.FindRecord(itm => itm.ActivatorInstanceId.HasValue && assignedQueueItemsCount.QueueIdsInSameStage.Contains(itm.QueueId));
                    if (assignedQueueItemInSameStage != null)
                    {
                        serviceToAssign = servicesToAssign.FindRecord(itm => itm.ServiceInstance.ServiceInstanceId == assignedQueueItemInSameStage.ActivatorInstanceId.Value);
                        if (serviceToAssign == null)
                            continue;
                    }
                    AssignServiceToQueueItem(pendingQueueItem, pendingQueueItemsToUpdate, assignedQueueItemsCount, ref serviceInstanceIndex, servicesToAssign, serviceToAssign);
                }

                NotifyServices(itemsCountByServiceId, pendingQueueItemsToUpdate);
            }
        }

        private void BuildItemsCountObjs(List<Runtime.Entities.RuntimeServiceInstance> activeServiceInstances, List<PendingQueueItemInfo> pendingQueueItems, 
            out Dictionary<int, AssignedQueueItemsCount> itemsCountByQueue, out Dictionary<Guid, ServiceInstanceItemsCount> itemsCountByService, out HashSet<int> sequentialQueueIds)
        {
            Dictionary<int, QueueRuntimeInfo> queueRuntimeInfos = _execFlowManager.GetQueueRuntimeInfoByQueueId();
            sequentialQueueIds = new HashSet<int>(queueRuntimeInfos.Where(itm => itm.Value.IsSequencial).Select(itm => itm.Key));
            itemsCountByQueue = new Dictionary<int, AssignedQueueItemsCount>();
            itemsCountByService = activeServiceInstances.ToDictionary(serviceInstance => serviceInstance.ServiceInstanceId,
                serviceInstance => new ServiceInstanceItemsCount
                {
                    ServiceInstance = serviceInstance,
                    QueueIds = new List<int>()
                });
            foreach (var pendingQueueItem in pendingQueueItems)
            {
                AssignedQueueItemsCount assignedQueueItemsCount = itemsCountByQueue.GetOrCreateItem(pendingQueueItem.QueueId,
                    () => new AssignedQueueItemsCount(pendingQueueItem.QueueId, queueRuntimeInfos));
                if (pendingQueueItem.ActivatorInstanceId.HasValue)
                {
                    ServiceInstanceItemsCount serviceItemsCount;
                    if (itemsCountByService.TryGetValue(pendingQueueItem.ActivatorInstanceId.Value, out serviceItemsCount))
                    {
                        serviceItemsCount.ItemsCount++;
                        serviceItemsCount.QueueIds.Add(pendingQueueItem.QueueId);
                        assignedQueueItemsCount.ItemsCount++;
                    }
                    else
                        pendingQueueItem.ActivatorInstanceId = null;
                }
            }
        }

        private void AssignServiceToQueueItem(PendingQueueItemInfo pendingQueueItem, List<PendingQueueItemInfo> pendingQueueItemsToUpdate, AssignedQueueItemsCount assignedQueueItemsCount, ref int serviceInstanceIndex, List<ServiceInstanceItemsCount> servicesToAssign, ServiceInstanceItemsCount serviceToAssign = null)
        {
            if (serviceToAssign == null)
            {
                if (serviceInstanceIndex >= servicesToAssign.Count)
                    serviceInstanceIndex = 0;

                serviceToAssign = servicesToAssign[serviceInstanceIndex];
            }

            pendingQueueItem.ActivatorInstanceId = serviceToAssign.ServiceInstance.ServiceInstanceId;
            pendingQueueItemsToUpdate.Add(pendingQueueItem);

            assignedQueueItemsCount.ItemsCount++;
            serviceToAssign.QueueIds.Add(pendingQueueItem.QueueId);
            serviceToAssign.ItemsCount++;

            if (serviceToAssign.ItemsCount >= s_maxNumberOfItemsPerService)
                servicesToAssign.Remove(serviceToAssign);

            serviceInstanceIndex++;
        }

        private void AssignSummaryBatchesToServices(List<Runtime.Entities.RuntimeServiceInstance> activeServiceInstances)
        {
            if (activeServiceInstances == null || activeServiceInstances.Count == 0)
                return;

            HoldRequestManager holdRequestManager = new HoldRequestManager();
            IOrderedEnumerable<HoldRequest> holdRequests = holdRequestManager.GetCachedOrderedHoldRequests();

            List<SummaryBatch> summaryBatches = _queueItemManager.GetSummaryBatches(holdRequests);
            if (summaryBatches != null && summaryBatches.Count > 0)
            {
                Dictionary<int, QueueRuntimeInfo> queueRuntimeInfosByQueueId = _execFlowManager.GetQueueRuntimeInfoByQueueId();

                List<SummaryBatchActivator> assignedSummaryBatchActivators = _summaryBatchActivatorDataManager.GetAllSummaryBatchActivators();

                Dictionary<int, AssignedQueueItemsCount> assignedBatchesCountByQueue = new Dictionary<int, AssignedQueueItemsCount>();
                Dictionary<Guid, ServiceInstanceItemsCount> itemsCountByServiceId
                    = activeServiceInstances.ToDictionary(serviceInstance => serviceInstance.ServiceInstanceId, serviceInstance => new ServiceInstanceItemsCount { ServiceInstance = serviceInstance });

                for (int i = assignedSummaryBatchActivators.Count - 1; i >= 0; i--)
                {
                    var summaryBatchActivator = assignedSummaryBatchActivators[i];
                    AssignedQueueItemsCount assignedQueueItemsCount = assignedBatchesCountByQueue.GetOrCreateItem(summaryBatchActivator.QueueId, 
                        () => new AssignedQueueItemsCount (summaryBatchActivator.QueueId, queueRuntimeInfosByQueueId)
                    );
                    ServiceInstanceItemsCount serviceItemsCount;
                    if (itemsCountByServiceId.TryGetValue(summaryBatchActivator.ActivatorId, out serviceItemsCount))
                    {
                        serviceItemsCount.ItemsCount++;
                        assignedQueueItemsCount.ItemsCount++;
                    }
                    else
                    {
                        _summaryBatchActivatorDataManager.Delete(summaryBatchActivator.QueueId, summaryBatchActivator.BatchStart);
                        assignedSummaryBatchActivators.Remove(summaryBatchActivator);
                    }
                }
                List<SummaryBatchActivator> finalSummaryBatchActivators = new List<SummaryBatchActivator>(assignedSummaryBatchActivators);
                List<SummaryBatchActivator> summaryBatchActivatorsToCreate = new List<SummaryBatchActivator>();
                List<ServiceInstanceItemsCount> servicesToAssign = itemsCountByServiceId.Values.Where(itm => itm.ItemsCount < s_maxNumberOfItemsPerService).ToList();
                
                foreach (var summarybatch in summaryBatches.OrderBy(itm => itm.BatchStart))
                {
                    if (servicesToAssign.Count == 0)
                        break;

                    if (finalSummaryBatchActivators.Any(itm => itm.QueueId == summarybatch.QueueId && itm.BatchStart == summarybatch.BatchStart))//if already assigned
                        continue;

                    AssignedQueueItemsCount assignedQueueItemsCount = assignedBatchesCountByQueue.GetOrCreateItem(summarybatch.QueueId,
                        () => new AssignedQueueItemsCount(summarybatch.QueueId, queueRuntimeInfosByQueueId));
                    if (assignedQueueItemsCount.ItemsCount >= assignedQueueItemsCount.MaxItemsCount)
                        continue;

                    ServiceInstanceItemsCount serviceToAssign = null;
                    var assignedBatchActivatorInSameStage = finalSummaryBatchActivators.FindRecord(itm => itm.BatchStart == summarybatch.BatchStart && assignedQueueItemsCount.QueueIdsInSameStage.Contains(summarybatch.QueueId));
                    if (assignedBatchActivatorInSameStage != null)
                    {
                        serviceToAssign = servicesToAssign.FindRecord(itm => itm.ServiceInstance.ServiceInstanceId == assignedBatchActivatorInSameStage.ActivatorId);
                    }
                    else
                    {
                        serviceToAssign = servicesToAssign.OrderBy(srv => srv.ItemsCount).First();
                    }

                    if (serviceToAssign != null)
                    {
                        var summaryBatchActivator = new SummaryBatchActivator
                        {
                            QueueId = summarybatch.QueueId,
                            BatchStart = summarybatch.BatchStart,
                            ActivatorId = serviceToAssign.ServiceInstance.ServiceInstanceId,
                            BatchEnd = summarybatch.BatchEnd
                        };
                        summaryBatchActivatorsToCreate.Add(summaryBatchActivator);
                        finalSummaryBatchActivators.Add(summaryBatchActivator);

                        assignedQueueItemsCount.ItemsCount++;
                        serviceToAssign.ItemsCount++;
                        if (serviceToAssign.ItemsCount >= s_maxNumberOfItemsPerService)
                            servicesToAssign.Remove(serviceToAssign);
                    }
                }
                _summaryBatchActivatorDataManager.Insert(summaryBatchActivatorsToCreate);
                NotifySummaryServices(itemsCountByServiceId);
            }
        }

        private void NotifyServices(Dictionary<Guid, ServiceInstanceItemsCount> itemsCountByServiceId, List<PendingQueueItemInfo> pendingQueueItemsToUpdate)
        {
            if (pendingQueueItemsToUpdate != null && pendingQueueItemsToUpdate.Count > 0)
                _queueItemDataManager.SetQueueItemsActivatorInstances(pendingQueueItemsToUpdate);

            var interRuntimeServiceManager = new InterRuntimeServiceManager();

            Parallel.ForEach(itemsCountByServiceId.Values.Where(itm => itm.ItemsCount > 0),
                (serviceInstanceItemsCount) =>
                {
                    InterServicePendingItemsRequest request = new InterServicePendingItemsRequest
                    {
                        ServiceInstanceId = serviceInstanceItemsCount.ServiceInstance.ServiceInstanceId,
                        QueueIds = serviceInstanceItemsCount.QueueIds.Distinct().ToList()
                    };
                    try
                    {
                        interRuntimeServiceManager.SendRequest(serviceInstanceItemsCount.ServiceInstance.ProcessId, request);
                    }
                    catch (Exception ex)
                    {
                        LoggerFactory.GetExceptionLogger().WriteException(ex);
                    }
                });
        }

        private void NotifySummaryServices(Dictionary<Guid, ServiceInstanceItemsCount> itemsCountByServiceId)
        {            
            if (itemsCountByServiceId.Any(itm => itm.Value.ItemsCount > 0))
            {
                var interRuntimeServiceManager = new InterRuntimeServiceManager();

                Parallel.ForEach(itemsCountByServiceId.Values.Where(itm => itm.ItemsCount > 0),
                    (serviceInstanceItemsCount) =>
                    {
                        InterServicePendingSummaryBatchesRequest request = new InterServicePendingSummaryBatchesRequest
                        {
                            ServiceInstanceId = serviceInstanceItemsCount.ServiceInstance.ServiceInstanceId
                        };
                        try
                        {
                            interRuntimeServiceManager.SendRequest(serviceInstanceItemsCount.ServiceInstance.ProcessId, request);
                        }
                        catch (Exception ex)
                        {
                            LoggerFactory.GetExceptionLogger().WriteException(ex);
                        }
                    });
            }
        }


        #region Private Classes

        private class AssignedQueueItemsCount
        {
            static QueueInstanceManager s_queueManager = new QueueInstanceManager();
            public AssignedQueueItemsCount(int queueId, Dictionary<int, QueueRuntimeInfo> queueRuntimeInfosByQueueId)
            {
                this.QueueId = queueId;
                var queueInstance = s_queueManager.GetQueueInstanceById(queueId);
                queueInstance.ThrowIfNull("queueInstance", queueId);
                this.MaxItemsCount = queueInstance.Settings.Activator.NbOfMaxConcurrentActivators;
                QueueRuntimeInfo queueRuntimeInfo = queueRuntimeInfosByQueueId.GetRecord(queueId);
                queueRuntimeInfo.ThrowIfNull("queueRuntimeInfo", queueId);
                this.QueueIdsInSameStage = queueRuntimeInfo.QueueIdsInSameStage != null ? new HashSet<int>(queueRuntimeInfo.QueueIdsInSameStage) : new HashSet<int>();

            }
            public int QueueId { get; private set; }

            public int ItemsCount { get; set; }

            public int MaxItemsCount { get; private set; }

            public HashSet<int> QueueIdsInSameStage { get; private set; }
        }

        private class ServiceInstanceItemsCount
        {
            public Runtime.Entities.RuntimeServiceInstance ServiceInstance { get; set; }

            public int ItemsCount { get; set; }

            public List<int> QueueIds { get; set; }
        }

        #endregion

        public int serviceInstanceIndex { get; set; }
    }
}