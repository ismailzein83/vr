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
using System.Configuration;
using Vanrise.Security.Business;

namespace Vanrise.BusinessProcess
{
    public class BPRegulatorRuntimeService : RuntimeService
    {
        static int s_defaultMaxConcurrentWorkflowsPerDefinition = BPDefinitionInitiator.s_defaultMaxConcurrentWorkflowsPerDefinition;
        static int s_maxWorkflowsPerServiceInstance;
        static int s_moreAssignableItemsToMaxConcurrentPerBPDefinition;

        BPDefinitionManager _bpDefinitionManager = new BPDefinitionManager();
        IBPInstanceDataManager _bpInstanceDataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
        IBPEventDataManager _bpEventDataManager = BPDataManagerFactory.GetDataManager<IBPEventDataManager>();
        RuntimeServiceInstanceManager _serviceInstanceManager = new RuntimeServiceInstanceManager();
        UserManager _userManager = new UserManager();

        static BPRegulatorRuntimeService()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["BusinessProcess_MaxWorkflowsPerServiceInstance"], out s_maxWorkflowsPerServiceInstance))
                s_maxWorkflowsPerServiceInstance = 50;
            if (!int.TryParse(ConfigurationManager.AppSettings["BusinessProcess_MoreAssignableItemsToMaxConcurrentPerBPDefinition"], out s_moreAssignableItemsToMaxConcurrentPerBPDefinition))
                s_moreAssignableItemsToMaxConcurrentPerBPDefinition = 0;
        }

        protected override void Execute()
        {
            TransactionLocker.Instance.TryLock("BPRegulatorRuntimeService_Execute", () =>
            {
                var bpServiceInstances = GetRunningServiceInstances();
                if (bpServiceInstances == null || bpServiceInstances.Count == 0)
                    return;
                AssignPendingInstancesToServices(bpServiceInstances);
            });
        }

        private List<Runtime.Entities.RuntimeServiceInstance> GetRunningServiceInstances()
        {
            return _serviceInstanceManager.GetServices(BusinessProcessService.SERVICE_TYPE_UNIQUE_NAME);
        }

        static List<BPInstanceStatus> s_pendingStatuses = BPInstanceStatusAttribute.GetNonClosedStatuses();

        private void AssignPendingInstancesToServices(List<RuntimeServiceInstance> bpServiceInstances)
        {
            var pendingInstances = _bpInstanceDataManager.GetPendingInstancesInfo(s_pendingStatuses);
            if (pendingInstances != null && pendingInstances.Count > 0)
            {
                Dictionary<long, BPInstance> pendingInstancesDict = pendingInstances.ToDictionary(itm => itm.ProcessInstanceID, itm => itm);
                List<long> processInstanceIdsHavingEvents = _bpEventDataManager.GetEventsDistinctProcessInstanceIds();
                Dictionary<Guid, BPDefinition> bpDefinitions = _bpDefinitionManager.GetCachedBPDefinitions();
                Dictionary<Guid, ServiceInstanceBPDefinitionInfo> serviceInstancesInfo
                    = bpServiceInstances.ToDictionary(serviceInstance => serviceInstance.ServiceInstanceId,
                    serviceInstance => new ServiceInstanceBPDefinitionInfo
                    {
                        ServiceInstance = serviceInstance,
                        ItemsCountByBPDefinition = BuildItemsCountByBPDefinition(bpDefinitions)
                    });
                List<BPInstance> startedBPInstances = new List<BPInstance>();
                List<BPInstance> waitingAndNotPersistedInstances = new List<BPInstance>();
                List<BPInstance> persistedInstances = new List<BPInstance>();
                List<BPInstance> newInstances = new List<BPInstance>();
                List<BPInstanceCancellationRequestInfo> instanceCancellationRequests = new List<BPInstanceCancellationRequestInfo>();
                // - handling BPInstances assigned to Services and scheduled for execution
                // - distribute other BPInstances to corresponding lists (waitingAndNotPersistedInstances, newAndPersistedInstances)
                // - suspend instances where applicable
                foreach (var pendingInstanceInfo in pendingInstances.OrderBy(itm => itm.ProcessInstanceID))
                {
                    if (pendingInstanceInfo.ServiceInstanceId.HasValue)
                    {
                        //check if runtime service is still alive
                        ServiceInstanceBPDefinitionInfo serviceInstanceInfo;
                        if (serviceInstancesInfo.TryGetValue(pendingInstanceInfo.ServiceInstanceId.Value, out serviceInstanceInfo))
                        {
                            bool instanceStopped = StopInstanceIfNeeded(pendingInstanceInfo, pendingInstancesDict, instanceCancellationRequests);
                            if (pendingInstanceInfo.AssignmentStatus != BPInstanceAssignmentStatus.Free)//already assigned for execution
                            {
                                var bpDefinitionInfo = serviceInstanceInfo.GetBPDefinitionInfo(pendingInstanceInfo.DefinitionID);
                                serviceInstanceInfo.TotalItemsCount++;
                                bpDefinitionInfo.ItemsCount++;
                                if (pendingInstanceInfo.AssignmentStatus != BPInstanceAssignmentStatus.Executing)//execution might not be started yet
                                {
                                    bpDefinitionInfo.HasAnyNewInstance = true;
                                    serviceInstanceInfo.HasAnyNewInstance = true;
                                }
                            }
                            else//waiting event
                            {
                                if (!instanceStopped && processInstanceIdsHavingEvents.Contains(pendingInstanceInfo.ProcessInstanceID))
                                    waitingAndNotPersistedInstances.Add(pendingInstanceInfo);
                            }
                            startedBPInstances.Add(pendingInstanceInfo);
                        }
                        else//runtime service not alive
                        {
                            StopPendingInstance(pendingInstanceInfo, pendingInstancesDict, true, true, "Runtime is no longer available");
                        }
                    }
                    else//waiting event and persisted or not started yet
                    {
                        bool instanceStopped = StopInstanceIfNeeded(pendingInstanceInfo, pendingInstancesDict, instanceCancellationRequests);
                        if (!instanceStopped)
                        {
                            if (pendingInstanceInfo.Status == BPInstanceStatus.New || pendingInstanceInfo.Status == BPInstanceStatus.Postponed)
                            {
                                newInstances.Add(pendingInstanceInfo);
                            }
                            else//Waiting
                            {
                                if (processInstanceIdsHavingEvents.Contains(pendingInstanceInfo.ProcessInstanceID))
                                    persistedInstances.Add(pendingInstanceInfo);
                                startedBPInstances.Add(pendingInstanceInfo);
                            }
                        }
                    }                    
                }

                Dictionary<Guid, ServiceInstanceBPDefinitionInfo> servicesToAssign = serviceInstancesInfo.Values.Where(itm => itm.TotalItemsCount < s_maxWorkflowsPerServiceInstance).ToDictionary(itm => itm.ServiceInstance.ServiceInstanceId, itm => itm);
                List<BPInstance> pendingInstancesToUpdate = new List<BPInstance>();

                //handling waiting BPInstances that are not persisted
                foreach (var pendingInstanceInfo in waitingAndNotPersistedInstances.OrderBy(itm => itm.ProcessInstanceID))
                {
                    if (servicesToAssign.Count == 0)
                        break;

                      var assignedServiceInstanceInfo = servicesToAssign.GetRecord(pendingInstanceInfo.ServiceInstanceId.Value);
                      if (assignedServiceInstanceInfo != null)
                          TryAssignServiceToBPInstance(pendingInstanceInfo, assignedServiceInstanceInfo, servicesToAssign, pendingInstancesToUpdate);                   
                }

                //handling waiting BPInstances that are persisted
                foreach (var pendingInstanceInfo in persistedInstances.OrderBy(itm => itm.ProcessInstanceID))
                {
                    if (servicesToAssign.Count == 0)
                        break;

                    TryAssignLeastBusyServiceToBPInstance(pendingInstanceInfo, servicesToAssign, pendingInstancesToUpdate);
                }

                Func<List<BPInstance>> getStartedBPInstances = () =>
                    {
                        return startedBPInstances;
                    };

                //handling new BPInstances
                foreach (var pendingInstanceInfo in newInstances.OrderByDescending(itm => itm.Status).ThenBy(itm => itm.ProcessInstanceID))//OrderByDescending(itm => itm.Status) to have postponed instances then new
                {
                    if (servicesToAssign.Count == 0)
                        break;

                    BPDefinitionExtendedSettings definitionExtendedSettings;
                    var bpDefinition = FindBPDefinitionWithValidate(bpDefinitions, pendingInstanceInfo.DefinitionID, out definitionExtendedSettings);
                    var canRunBPInstanceContext = new BPDefinitionCanRunBPInstanceContext(pendingInstanceInfo, getStartedBPInstances);
                    if (definitionExtendedSettings.CanRunBPInstance(canRunBPInstanceContext))
                    {
                        if (TryAssignLeastBusyServiceToBPInstance(pendingInstanceInfo, servicesToAssign, pendingInstancesToUpdate))
                            startedBPInstances.Add(pendingInstanceInfo);
                    }
                    else
                    {
                        if (canRunBPInstanceContext.Reason != null && canRunBPInstanceContext.Reason != pendingInstanceInfo.LastMessage)
                        {
                            BPTrackingChannel.Current.WriteTrackingMessage(new BPTrackingMessage
                            {
                                ProcessInstanceId = pendingInstanceInfo.ProcessInstanceID,
                                ParentProcessId = pendingInstanceInfo.ParentProcessID,
                                Severity = Vanrise.Entities.LogEntryType.Warning,
                                TrackingMessage = canRunBPInstanceContext.Reason,
                                EventTime = DateTime.Now
                            });
                            _bpInstanceDataManager.UpdateInstanceLastMessage(pendingInstanceInfo.ProcessInstanceID, canRunBPInstanceContext.Reason);
                        }
                        if (pendingInstanceInfo.Status != BPInstanceStatus.Postponed)
                        {
                            pendingInstanceInfo.Status = BPInstanceStatus.Postponed;
                            BPDefinitionInitiator.UpdateProcessStatus(pendingInstanceInfo.ProcessInstanceID, pendingInstanceInfo.ParentProcessID, pendingInstanceInfo.Status, BPInstanceAssignmentStatus.Free, null, null);
                        }
                    }
                }

                if (pendingInstancesToUpdate.Count > 0)
                    _bpInstanceDataManager.UpdateServiceInstancesAndAssignmentStatus(pendingInstancesToUpdate);

                if (instanceCancellationRequests.Count > 0)
                    SendCancellationRequests(serviceInstancesInfo, instanceCancellationRequests);
                NotifyServiceInstances(serviceInstancesInfo);

                //delete BPEvents for BPInstances that are not running
                if (processInstanceIdsHavingEvents.Count > 0)
                {
                    foreach(var processInstanceId in processInstanceIdsHavingEvents)
                    {
                        BPInstance processInstance;
                        if (!pendingInstancesDict.TryGetValue(processInstanceId, out processInstance) || processInstance.Status == BPInstanceStatus.Suspended || processInstance.Status == BPInstanceStatus.Cancelled)
                            _bpEventDataManager.DeleteProcessInstanceEvents(processInstanceId);
                    }
                }
            }
        }

        private bool StopInstanceIfNeeded(BPInstance pendingInstanceInfo, Dictionary<long, BPInstance> pendingInstancesDict, List<BPInstanceCancellationRequestInfo> instanceCancellationRequests)
        {
            bool shouldStopInstance = false;
            bool suspend = false;
            bool notifyParent = false;
            string reason = null;
            if (pendingInstanceInfo.Status != BPInstanceStatus.Cancelling)
            {
                if (pendingInstanceInfo.CancellationRequestByUserId.HasValue)
                {
                    shouldStopInstance = true;
                    suspend = false;
                    reason = String.Format("Cancellation requested by User '{0}'", _userManager.GetUserName(pendingInstanceInfo.CancellationRequestByUserId.Value));
                    notifyParent = true;
                }
                else if (pendingInstanceInfo.ParentProcessID.HasValue)
                {
                    BPInstance parentBPInstance;
                    pendingInstancesDict.TryGetValue(pendingInstanceInfo.ParentProcessID.Value, out parentBPInstance);
                    if (parentBPInstance == null || (parentBPInstance.Status != BPInstanceStatus.Running && parentBPInstance.Status != BPInstanceStatus.Waiting))
                    {
                        shouldStopInstance = true;
                        if (parentBPInstance != null && parentBPInstance.Status == BPInstanceStatus.Cancelled)
                        {
                            suspend = false;
                            reason = "Parent Process is cancelled";
                        }
                        else
                        {
                            suspend = true;
                            reason = "Parent Process is not running anymore";
                        }
                        notifyParent = false;                        
                    }
                }
                if (shouldStopInstance)
                {
                    if(pendingInstanceInfo.ServiceInstanceId.HasValue)
                    {
                        instanceCancellationRequests.Add(new BPInstanceCancellationRequestInfo
                        {
                            BPInstance = pendingInstanceInfo,
                            Reason = reason
                        });
                    }
                    else
                    {
                        StopPendingInstance(pendingInstanceInfo, pendingInstancesDict, suspend, notifyParent, reason);
                    }
                }
            }
            return shouldStopInstance;
        }

        private bool TryAssignLeastBusyServiceToBPInstance(BPInstance pendingInstanceInfo, Dictionary<Guid, ServiceInstanceBPDefinitionInfo> servicesToAssign, List<BPInstance> pendingInstancesToUpdate)
        {
            foreach (var serviceInstanceInfo in servicesToAssign.Values.OrderBy(itm => itm.TotalItemsCount))
            {
                if (TryAssignServiceToBPInstance(pendingInstanceInfo, serviceInstanceInfo, servicesToAssign, pendingInstancesToUpdate))
                    return true;
            }
            return false;
        }

        private bool TryAssignServiceToBPInstance(BPInstance pendingInstanceInfo, ServiceInstanceBPDefinitionInfo serviceInstanceInfo, Dictionary<Guid, ServiceInstanceBPDefinitionInfo> servicesToAssign, List<BPInstance> pendingInstancesToUpdate)
        {
            if (pendingInstanceInfo.AssignmentStatus != BPInstanceAssignmentStatus.Free)
                throw new Exception(String.Format("Invalid Assignment Status '{0}' for pendingInstanceInfo Id '{1}'", pendingInstanceInfo.AssignmentStatus.ToString(), pendingInstanceInfo.ProcessInstanceID));
            var bpDefinitionInfo = serviceInstanceInfo.GetBPDefinitionInfo(pendingInstanceInfo.DefinitionID);
            int maxInstancesPerBPDefinition = bpDefinitionInfo.BPDefinition.Configuration != null && bpDefinitionInfo.BPDefinition.Configuration.MaxConcurrentWorkflows.HasValue ?
                            bpDefinitionInfo.BPDefinition.Configuration.MaxConcurrentWorkflows.Value : s_defaultMaxConcurrentWorkflowsPerDefinition;
            maxInstancesPerBPDefinition += s_moreAssignableItemsToMaxConcurrentPerBPDefinition;
            if (bpDefinitionInfo.ItemsCount < maxInstancesPerBPDefinition)
            {
                pendingInstanceInfo.ServiceInstanceId = serviceInstanceInfo.ServiceInstance.ServiceInstanceId;
                pendingInstanceInfo.AssignmentStatus = BPInstanceAssignmentStatus.AssignedForExecution;
                pendingInstancesToUpdate.Add(pendingInstanceInfo);
                bpDefinitionInfo.ItemsCount++;
                serviceInstanceInfo.TotalItemsCount++;
                bpDefinitionInfo.HasAnyNewInstance = true;
                serviceInstanceInfo.HasAnyNewInstance = true;
                if (serviceInstanceInfo.TotalItemsCount >= s_maxWorkflowsPerServiceInstance)
                    servicesToAssign.Remove(serviceInstanceInfo.ServiceInstance.ServiceInstanceId);
                return true;
            }
            else
            {
                return false;
            }
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

        private void SendCancellationRequests(Dictionary<Guid, ServiceInstanceBPDefinitionInfo> serviceInstancesInfo, List<BPInstanceCancellationRequestInfo> instanceCancellationRequests)
        {
            var interRuntimeServiceManager = new InterRuntimeServiceManager();
            foreach (var cancellationRequestInfo in instanceCancellationRequests)
            {
                if (!cancellationRequestInfo.BPInstance.ServiceInstanceId.HasValue)
                    throw new NullReferenceException(String.Format("cancellationRequestInfo.BPInstance.ServiceInstanceId. BPInstanceId '{0}'", cancellationRequestInfo.BPInstance.ProcessInstanceID));
                var serviceInstanceInfo = serviceInstancesInfo.GetRecord(cancellationRequestInfo.BPInstance.ServiceInstanceId.Value);
                serviceInstanceInfo.ThrowIfNull("serviceInstanceInfo", cancellationRequestInfo.BPInstance.ServiceInstanceId.Value);
                InterBPServiceCancelInstanceRequest request = new InterBPServiceCancelInstanceRequest
                {
                    ServiceInstanceId = serviceInstanceInfo.ServiceInstance.ServiceInstanceId,
                    CancellationRequest = new BPInstanceCancellationRequest
                    {
                        BPDefinitionId = cancellationRequestInfo.BPInstance.DefinitionID,
                        BPInstanceId = cancellationRequestInfo.BPInstance.ProcessInstanceID,
                        Reason = cancellationRequestInfo.Reason
                    }
                };
                try
                {
                    interRuntimeServiceManager.SendRequest(serviceInstanceInfo.ServiceInstance.ProcessId, request);
                }
                catch (Exception ex)
                {
                    LoggerFactory.GetExceptionLogger().WriteException(ex);
                }
            }
        }

        private void StopPendingInstance(BPInstance pendingInstanceInfo, Dictionary<long, BPInstance> pendingInstancesDict, bool isSuspend, bool notifyParentIfAny, string errorMessage)
        {
            BPTrackingChannel.Current.WriteTrackingMessage(new BPTrackingMessage
                {
                    ProcessInstanceId = pendingInstanceInfo.ProcessInstanceID,
                    ParentProcessId = pendingInstanceInfo.ParentProcessID,
                    Severity = Vanrise.Entities.LogEntryType.Error,
                    TrackingMessage = errorMessage,
                    EventTime = DateTime.Now
                });
            var status = isSuspend ? BPInstanceStatus.Suspended : BPInstanceStatus.Cancelled;
            BPDefinitionInitiator.UpdateProcessStatus(pendingInstanceInfo.ProcessInstanceID, pendingInstanceInfo.ParentProcessID, status, BPInstanceAssignmentStatus.Free, errorMessage, null);
            pendingInstanceInfo.Status = status;
            if (notifyParentIfAny && pendingInstanceInfo.ParentProcessID.HasValue)
            {
                BPInstance parentBPInstance;
                if (pendingInstancesDict.TryGetValue(pendingInstanceInfo.ParentProcessID.Value, out parentBPInstance) && parentBPInstance.Status != BPInstanceStatus.Suspended && parentBPInstance.Status != BPInstanceStatus.Cancelled)
                {
                    BPDefinitionInitiator.NotifyParentBPChildCompleted(pendingInstanceInfo.ProcessInstanceID, pendingInstanceInfo.ParentProcessID.Value,
                        status, errorMessage, null, null);
                }
            }
            //if (pendingInstanceInfo.CompletionNotifier != null)
            BPDefinitionInitiator.NotifyBPCompleted(pendingInstanceInfo, null);
        }

        private BPDefinition FindBPDefinitionWithValidate(Dictionary<Guid, BPDefinition> bpDefinitions, Guid bpDefinitionId, out BPDefinitionExtendedSettings definitionExtendedSettings)
        {
            BPDefinition definition;
            if (!bpDefinitions.TryGetValue(bpDefinitionId, out definition))
                throw new NullReferenceException(String.Format("definition '{0}'", bpDefinitionId));
            definitionExtendedSettings = _bpDefinitionManager.GetBPDefinitionExtendedSettings(definition);
            definitionExtendedSettings.ThrowIfNull("definitionExtendedSettings", bpDefinitionId);
            return definition;
        }

        private Dictionary<Guid, BPDefinitionInfo> BuildItemsCountByBPDefinition(Dictionary<Guid, BPDefinition> bpDefinitions)
        {
            return bpDefinitions.Values.ToDictionary(bpDefinition => bpDefinition.BPDefinitionID
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

        public class BPDefinitionCanRunBPInstanceContext : IBPDefinitionCanRunBPInstanceContext
        {
            Func<List<BPInstance>> _getStartedBPInstances;

            public BPDefinitionCanRunBPInstanceContext(BPInstance instanceToRun, Func<List<BPInstance>> getStartedBPInstances)
            {
                this.IntanceToRun = instanceToRun;
                this._getStartedBPInstances = getStartedBPInstances;
            }

            public BPInstance IntanceToRun
            {
                get;
                private set;
            }



            public string Reason { set; get; }

            public List<BPInstance> GetStartedBPInstances()
            {
                return _getStartedBPInstances();
            }
        }

        private class BPInstanceCancellationRequestInfo
        {
            public BPInstance BPInstance { get; set; }

            public string Reason { get; set; }
        }

        #endregion
    }
}
