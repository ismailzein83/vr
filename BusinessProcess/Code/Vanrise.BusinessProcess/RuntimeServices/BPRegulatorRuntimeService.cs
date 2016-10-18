using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Data;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Business;

namespace Vanrise.BusinessProcess
{
    public class BPRegulatorRuntimeService : RuntimeService
    {
        static int s_maxWorkflowsPerServiceInstance = 5;
        static int s_maxConcurrentWorkflowsPerDefinition = 5;
        static int s_moreAssignableItemsToMaxConcurrentPerBPDefinition = 0;

        BPDefinitionManager _bpDefinitionManager = new BPDefinitionManager();
        IBPInstanceDataManager _bpInstanceDataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
        IBPEventDataManager _bpEventDataManager = BPDataManagerFactory.GetDataManager<IBPEventDataManager>();
        RuntimeServiceInstanceManager _serviceInstanceManager = new RuntimeServiceInstanceManager();

        protected override void Execute()
        {
            TransactionLocker.Instance.TryLock("BPRegulatorRuntimeService_Execute", () =>
            {
                var bpServiceInstances = GetRunningServiceInstances();
                List<BPPendingInstanceInfo> runningInstances;
                AssignPendingInstancesToServices(bpServiceInstances, out runningInstances);
                if(runningInstances != null && runningInstances.Count > 0)
                {
                    CheckAndNotifyPendingBPEvents(bpServiceInstances, runningInstances);
                }
            });
        }

        private List<Runtime.Entities.RuntimeServiceInstance> GetRunningServiceInstances()
        {            
            return _serviceInstanceManager.GetServices(BusinessProcessService.SERVICE_TYPE_UNIQUE_NAME);
        }

        static List<BPInstanceStatus> s_pendingStatuses = new List<BPInstanceStatus> { BPInstanceStatus.New, BPInstanceStatus.Running };

        private void AssignPendingInstancesToServices(List<RuntimeServiceInstance> bpServiceInstances, out List<BPPendingInstanceInfo> runningInstances)
        {
            int nbOfInstancesToRetrieve = bpServiceInstances.Count * s_maxWorkflowsPerServiceInstance;
            var pendingInstancesInfo = _bpInstanceDataManager.GetPendingInstancesInfo(s_pendingStatuses, nbOfInstancesToRetrieve);
            if (pendingInstancesInfo != null && pendingInstancesInfo.Count > 0)
            {
                var bpDefinitionsList = _bpDefinitionManager.GetBPDefinitions().ToList();
                Dictionary<Guid, BPDefinition> bpDefinitions = bpDefinitionsList.ToDictionary(bpDefinition => bpDefinition.BPDefinitionID, bpDefinition => bpDefinition);
                Dictionary<Guid, ServiceInstanceBPDefinitionInfo> serviceInstancesInfo
                    = bpServiceInstances.ToDictionary(serviceInstance => serviceInstance.ServiceInstanceId,
                    serviceInstance => new ServiceInstanceBPDefinitionInfo
                    {
                        ServiceInstance = serviceInstance,
                        ItemsCountByBPDefinition = BuildItemsCountByBPDefinition(bpDefinitionsList)
                    });
                List<BPPendingInstanceInfo> terminatedPendingInstances = new List<BPPendingInstanceInfo>();
                foreach (var pendingInstanceInfo in pendingInstancesInfo.OrderBy(itm => itm.ProcessInstanceId))
                {
                    if (pendingInstanceInfo.ServiceInstanceId.HasValue)
                    {
                        ServiceInstanceBPDefinitionInfo serviceInstanceInfo;
                        if (serviceInstancesInfo.TryGetValue(pendingInstanceInfo.ServiceInstanceId.Value, out serviceInstanceInfo))
                        {
                            serviceInstanceInfo.TotalItemsCount++;
                            var bpDefinitionInfo = serviceInstanceInfo.GetBPDefinitionInfo(pendingInstanceInfo.BPDefinitionId);
                            bpDefinitionInfo.ItemsCount++;
                            if (pendingInstanceInfo.Status == BPInstanceStatus.New)
                            {
                                bpDefinitionInfo.HasAnyNewInstance = true;
                                serviceInstanceInfo.HasAnyNewInstance = true;
                            }
                        }
                        else
                        {
                            SuspendPendingInstance(pendingInstanceInfo, pendingInstancesInfo, terminatedPendingInstances, true);
                        }
                    }
                    else if (pendingInstanceInfo.ParentProcessInstanceId.HasValue)
                    {
                        long parentProcessInstanceId = pendingInstanceInfo.ParentProcessInstanceId.Value;
                        if (!pendingInstancesInfo.Any(pendingInstance => pendingInstance.ProcessInstanceId == parentProcessInstanceId)
                            || terminatedPendingInstances.Any(pendingInstance => pendingInstance.ProcessInstanceId == parentProcessInstanceId))
                        {
                            SuspendPendingInstance(pendingInstanceInfo, pendingInstancesInfo, terminatedPendingInstances, false, "Parent Process is not running anymore");
                        }
                    }
                }

                List<ServiceInstanceBPDefinitionInfo> servicesToAssign = serviceInstancesInfo.Values.Where(itm => itm.TotalItemsCount < s_maxWorkflowsPerServiceInstance).ToList();
                List<BPPendingInstanceInfo> pendingInstancesToUpdate = new List<BPPendingInstanceInfo>();

                foreach (var pendingInstanceInfo in pendingInstancesInfo.Where(itm => !itm.ServiceInstanceId.HasValue && !terminatedPendingInstances.Contains(itm)).OrderBy(itm => itm.ProcessInstanceId))
                {
                    if (servicesToAssign.Count == 0)
                        break;

                    var bpDefinition = FindBPDefinition(bpDefinitions, pendingInstanceInfo.BPDefinitionId);
                    int maxInstancesPerBPDefinition = bpDefinition.Configuration != null && bpDefinition.Configuration.MaxConcurrentWorkflows.HasValue ?
                        bpDefinition.Configuration.MaxConcurrentWorkflows.Value : s_maxConcurrentWorkflowsPerDefinition;
                    maxInstancesPerBPDefinition += s_moreAssignableItemsToMaxConcurrentPerBPDefinition;

                    foreach (var serviceInstanceInfo in servicesToAssign.OrderBy(itm => itm.TotalItemsCount))
                    {
                        var bpDefinitionInfo = serviceInstanceInfo.GetBPDefinitionInfo(pendingInstanceInfo.BPDefinitionId);
                        if (bpDefinitionInfo.ItemsCount < maxInstancesPerBPDefinition)
                        {
                            pendingInstanceInfo.ServiceInstanceId = serviceInstanceInfo.ServiceInstance.ServiceInstanceId;
                            pendingInstancesToUpdate.Add(pendingInstanceInfo);
                            bpDefinitionInfo.ItemsCount++;
                            serviceInstanceInfo.TotalItemsCount++;
                            if (pendingInstanceInfo.Status == BPInstanceStatus.New)
                            {
                                bpDefinitionInfo.HasAnyNewInstance = true;
                                serviceInstanceInfo.HasAnyNewInstance = true;
                            }
                            if (serviceInstanceInfo.TotalItemsCount >= s_maxWorkflowsPerServiceInstance)
                                servicesToAssign.Remove(serviceInstanceInfo);
                            break;
                        }
                    };
                }

                if (pendingInstancesToUpdate.Count > 0)
                    _bpInstanceDataManager.SetServiceInstancesOfBPInstances(pendingInstancesToUpdate);
                NotifyServiceInstances(serviceInstancesInfo);
                runningInstances = pendingInstancesInfo.Where(itm => !terminatedPendingInstances.Contains(itm) && itm.Status == BPInstanceStatus.Running).ToList();
            }
            else
                runningInstances = null;
        }

        private void NotifyServiceInstances(Dictionary<Guid, ServiceInstanceBPDefinitionInfo> serviceInstancesInfo)
        {
            var interRuntimeServiceManager = new InterRuntimeServiceManager();

            Parallel.ForEach(serviceInstancesInfo.Values.Where(itm => itm.HasAnyNewInstance), (serviceInstanceInfo) =>
            {
                InterBPServicePendingInstancesRequest request = new InterBPServicePendingInstancesRequest
                {
                    ServiceInstanceId = serviceInstanceInfo.ServiceInstance.ServiceInstanceId,
                    PendingBPDefinitionIds = serviceInstanceInfo.ItemsCountByBPDefinition.Values.Where(bpDefinitionInfo => bpDefinitionInfo.HasAnyNewInstance).Select(bpDefinitionInfo => bpDefinitionInfo.BPDefinition.BPDefinitionID).ToList()
                };
                try
                {
                    interRuntimeServiceManager.SendRequest(serviceInstanceInfo.ServiceInstance.ProcessId, request);
                }
                catch (Exception ex)
                {
                    LoggerFactory.GetExceptionLogger().WriteException(ex);
                }
            });
        }

        private void SuspendPendingInstance(BPPendingInstanceInfo pendingInstanceInfo, List<BPPendingInstanceInfo> pendingInstancesInfo, List<BPPendingInstanceInfo> terminatedPendingInstances, bool notifyParentIfAny, string errorMessage = null)
        {
            if (errorMessage == null)
                errorMessage = "Runtime is no longer available";
            BPDefinitionInitiator.UpdateProcessStatus(pendingInstanceInfo.ProcessInstanceId, pendingInstanceInfo.ParentProcessInstanceId, BPInstanceStatus.Suspended, errorMessage, null);
            terminatedPendingInstances.Add(pendingInstanceInfo);
            if (pendingInstanceInfo.ParentProcessInstanceId.HasValue)
            {
                var parentInstanceInfo = pendingInstancesInfo.FirstOrDefault(itm => itm.ProcessInstanceId == pendingInstanceInfo.ParentProcessInstanceId.Value);
                if (parentInstanceInfo != null && parentInstanceInfo.Status == BPInstanceStatus.Running && !terminatedPendingInstances.Contains(parentInstanceInfo))
                {
                    BPDefinitionInitiator.NotifyParentBPChildCompleted(pendingInstanceInfo.ProcessInstanceId, pendingInstanceInfo.ParentProcessInstanceId.Value,
                        BPInstanceStatus.Suspended, errorMessage, null);
                }
            }
        }

        private void CheckAndNotifyPendingBPEvents(List<RuntimeServiceInstance> bpServiceInstances, List<BPPendingInstanceInfo> runningInstances)
        {            
            List<long> processInstanceIdsHavingEvents = _bpEventDataManager.GetEventsDistinctProcessInstanceIds();
            if(processInstanceIdsHavingEvents != null && processInstanceIdsHavingEvents.Count > 0)
            {
                Dictionary<Guid, HashSet<Guid>> bpDefinitionsIdsHavingEventsByServiceInstanceIds = new Dictionary<Guid, HashSet<Guid>>();
                foreach(var processInstanceId in processInstanceIdsHavingEvents)
                {
                    var matchRunningInstance = runningInstances.FirstOrDefault(itm => itm.ProcessInstanceId == processInstanceId);
                    if (matchRunningInstance != null)
                    {
                        if (matchRunningInstance.ServiceInstanceId.HasValue)
                        {
                            bpDefinitionsIdsHavingEventsByServiceInstanceIds.GetOrCreateItem(matchRunningInstance.ServiceInstanceId.Value).Add(matchRunningInstance.BPDefinitionId);
                        }
                    }
                    else
                        _bpEventDataManager.DeleteProcessInstanceEvents(processInstanceId);
                }

                if(bpDefinitionsIdsHavingEventsByServiceInstanceIds.Count > 0)
                {
                    var interRuntimeServiceManager = new InterRuntimeServiceManager();

                    Parallel.ForEach(bpDefinitionsIdsHavingEventsByServiceInstanceIds, (serviceInstanceInfo) =>
                    {
                        InterBPServicePendingEventsRequest request = new InterBPServicePendingEventsRequest
                        {
                            ServiceInstanceId = serviceInstanceInfo.Key,
                            PendingEventsBPDefinitionIds = serviceInstanceInfo.Value.ToList()
                        };
                        try
                        {
                            var serviceInstance = bpServiceInstances.FirstOrDefault(itm => itm.ServiceInstanceId == serviceInstanceInfo.Key);
                            if (serviceInstance == null)
                                throw new NullReferenceException(String.Format("serviceInstance '{0}'", serviceInstanceInfo.Key));
                            interRuntimeServiceManager.SendRequest(serviceInstance.ProcessId, request);
                        }
                        catch (Exception ex)
                        {
                            LoggerFactory.GetExceptionLogger().WriteException(ex);
                        }
                    });
                }
            }
        }

        private BPDefinition FindBPDefinition(Dictionary<Guid, BPDefinition> bpDefinitions, Guid bpDefinitionId)
        {
            BPDefinition definition;
            if (!bpDefinitions.TryGetValue(bpDefinitionId, out definition))
                throw new NullReferenceException(String.Format("definition '{0}'", bpDefinitionId));
            return definition;
        }

        private Dictionary<Guid, BPDefinitionInfo> BuildItemsCountByBPDefinition(List<BPDefinition> bpDefinitions)
        {
            return bpDefinitions.ToDictionary(bpDefinition => bpDefinition.BPDefinitionID
                , bpDefinition => new BPDefinitionInfo
                {
                    BPDefinition = bpDefinition
                });
        }

        #region Private Classes

        private class ServiceInstanceBPDefinitionInfo
        {
            public RuntimeServiceInstance ServiceInstance { get; set; }

            public int TotalItemsCount { get; set; }

            public bool HasAnyNewInstance { get; set; }

            public Dictionary<Guid, BPDefinitionInfo> ItemsCountByBPDefinition { get; set; }

            internal BPDefinitionInfo GetBPDefinitionInfo(Guid bpDefinitionId)
            {
                BPDefinitionInfo definitionInfo;
                if (!this.ItemsCountByBPDefinition.TryGetValue(bpDefinitionId, out definitionInfo))
                    throw new NullReferenceException(String.Format("definitionInfo '{0}'", bpDefinitionId));
                return definitionInfo;
            }
        }

        private class BPDefinitionInfo
        {
            public BPDefinition BPDefinition { get; set; }

            public bool HasAnyNewInstance { get; set; }

            public int ItemsCount { get; set; }
        }

        #endregion
    }
}
