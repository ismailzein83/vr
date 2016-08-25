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
        static int s_moreAssignableItemsToMaxConcurrentPerBPDefinition = 3;

        BPDefinitionManager _bpDefinitionManager = new BPDefinitionManager();
        IBPInstanceDataManager _bpInstanceDataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
        ServiceInstanceManager _serviceInstanceManager = new ServiceInstanceManager();

        protected override void Execute()
        {
            TransactionLocker.Instance.TryLock("BPRegulatorRuntimeService_Execute", () =>
            {
                var bpServiceInstances = GetRunningServiceInstances();
                AssignPendingInstancesToServices(bpServiceInstances);
            });
        }

        private List<Runtime.Entities.ServiceInstance> GetRunningServiceInstances()
        {            
            _serviceInstanceManager.DeleteNonRunningServices(BusinessProcessService.s_bpServiceInstanceType);
            return _serviceInstanceManager.GetServices(BusinessProcessService.s_bpServiceInstanceType);
        }

        static List<BPInstanceStatus> s_pendingStatuses = new List<BPInstanceStatus> { BPInstanceStatus.New, BPInstanceStatus.Running };

        private void AssignPendingInstancesToServices(List<ServiceInstance> bpServiceInstances)
        {
            var pendingInstancesInfo = _bpInstanceDataManager.GetPendingInstancesInfo(s_pendingStatuses);
            if (pendingInstancesInfo != null && pendingInstancesInfo.Count > 0)
            {
                var bpDefinitionsList = _bpDefinitionManager.GetBPDefinitions().ToList();
                Dictionary<int, BPDefinition> bpDefinitions = bpDefinitionsList.ToDictionary(bpDefinition => bpDefinition.BPDefinitionID, bpDefinition => bpDefinition);
                Dictionary<Guid, ServiceInstanceInfo> serviceInstancesInfo
                    = bpServiceInstances.ToDictionary(serviceInstance => serviceInstance.ServiceInstanceId,
                    serviceInstance => new ServiceInstanceInfo
                    {
                        ServiceInstance = serviceInstance,
                        ItemsCountByBPDefinition = BuildItemsCountByBPDefinition(bpDefinitionsList)
                    });
                List<BPPendingInstanceInfo> terminatedPendingInstances = new List<BPPendingInstanceInfo>();
                foreach (var pendingInstanceInfo in pendingInstancesInfo.OrderBy(itm => itm.ProcessInstanceId))
                {
                    if (pendingInstanceInfo.ServiceInstanceId.HasValue)
                    {
                        ServiceInstanceInfo serviceInstanceInfo;
                        if (serviceInstancesInfo.TryGetValue(pendingInstanceInfo.ServiceInstanceId.Value, out serviceInstanceInfo))
                        {
                            serviceInstanceInfo.TotalItemsCount++;
                            serviceInstanceInfo.GetBPDefinitionInfo(pendingInstanceInfo.BPDefinitionId).ItemsCount++;
                        }
                        else
                        {
                            SuspendPendingInstance(pendingInstanceInfo, pendingInstancesInfo, terminatedPendingInstances, true);
                        }
                    }
                    else if(pendingInstanceInfo.ParentProcessInstanceId.HasValue)
                    {
                        long parentProcessInstanceId = pendingInstanceInfo.ParentProcessInstanceId.Value;
                        if(!pendingInstancesInfo.Any(pendingInstance => pendingInstance.ProcessInstanceId == parentProcessInstanceId) 
                            || terminatedPendingInstances.Any(pendingInstance => pendingInstance.ProcessInstanceId == parentProcessInstanceId))
                        {
                            SuspendPendingInstance(pendingInstanceInfo, pendingInstancesInfo, terminatedPendingInstances, false, "Parent Process is not running anymore");
                        }
                    }
                }
                
                List<ServiceInstanceInfo> servicesToAssign = serviceInstancesInfo.Values.Where(itm => itm.TotalItemsCount < s_maxWorkflowsPerServiceInstance).ToList();
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
                            if (serviceInstanceInfo.TotalItemsCount >= s_maxWorkflowsPerServiceInstance)
                                servicesToAssign.Remove(serviceInstanceInfo);
                            break;
                        }
                    };
                }

                if (pendingInstancesToUpdate.Count > 0)
                    _bpInstanceDataManager.SetServiceInstancesOfBPInstances(pendingInstancesToUpdate);

                NotifyServiceInstances(serviceInstancesInfo);
            }
        }

        private void NotifyServiceInstances(Dictionary<Guid, ServiceInstanceInfo> serviceInstancesInfo)
        {
            var interRuntimeServiceManager = new InterRuntimeServiceManager();

            Parallel.ForEach(serviceInstancesInfo.Values.Where(itm => itm.TotalItemsCount > 0), (serviceInstanceInfo) =>
            {
                InterBPServiceRequest request = new InterBPServiceRequest
                {
                    ServiceInstanceId = serviceInstanceInfo.ServiceInstance.ServiceInstanceId,
                    PendingBPDefinitionIds = serviceInstanceInfo.ItemsCountByBPDefinition.Values.Where(bpDefinitionInfo => bpDefinitionInfo.ItemsCount > 0).Select(bpDefinitionInfo => bpDefinitionInfo.BPDefinition.BPDefinitionID).ToList()
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

        private BPDefinition FindBPDefinition(Dictionary<int, BPDefinition> bpDefinitions, int bpDefinitionId)
        {
            BPDefinition definition;
            if (!bpDefinitions.TryGetValue(bpDefinitionId, out definition))
                throw new NullReferenceException(String.Format("definition '{0}'", bpDefinitionId));
            return definition;
        }

        private Dictionary<int, BPDefinitionInfo> BuildItemsCountByBPDefinition(List<BPDefinition> bpDefinitions)
        {
            return bpDefinitions.ToDictionary(bpDefinition => bpDefinition.BPDefinitionID
                , bpDefinition => new BPDefinitionInfo
                {
                    BPDefinition = bpDefinition
                });
        }

        #region Private Classes

        private class ServiceInstanceInfo
        {
            public ServiceInstance ServiceInstance { get; set; }

            public int TotalItemsCount { get; set; }

            public Dictionary<int, BPDefinitionInfo> ItemsCountByBPDefinition { get; set; }

            internal BPDefinitionInfo GetBPDefinitionInfo(int bpDefinitionId)
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

            public int ItemsCount { get; set; }
        }

        #endregion
    }
}
