using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;
using Vanrise.Runtime.Business;
using Vanrise.Runtime.Entities;
using Vanrise.Security.Business;

namespace Vanrise.BusinessProcess
{
    public class BPRegulatorRuntimeService : RuntimeService
    {
        public override Guid ConfigId { get { return new Guid("FD6520DB-26DC-4473-BB7E-1F583BEC3A19"); } }

        static int s_defaultMaxConcurrentWorkflowsPerDefinition = BPDefinitionInitiator.s_defaultMaxConcurrentWorkflowsPerDefinition;
        static int s_maxWorkflowsPerServiceInstance;
        static int s_moreAssignableItemsToMaxConcurrentPerBPDefinition;

        static int s_archiveBeforeNbOfDays;
        static int s_nbOfInstancesToArchivePerQuery;

        UserManager _userManager = new UserManager();
        BPDefinitionManager _bpDefinitionManager = new BPDefinitionManager();
        BPInstanceManager _bpInstanceManager = new BPInstanceManager();
        ProcessSynchronisationManager _processSynchronisationManager = new ProcessSynchronisationManager();
        RuntimeServiceInstanceManager _serviceInstanceManager = new RuntimeServiceInstanceManager();
        HoldRequestManager _holdRequestManager = new HoldRequestManager();
        QueueExecutionFlowDefinitionManager _queueExecutionFlowDefinitionManager = new QueueExecutionFlowDefinitionManager();
        IBPEventDataManager _bpEventDataManager = BPDataManagerFactory.GetDataManager<IBPEventDataManager>();
        SchedulerTaskManager _schedulerTaskManager = new SchedulerTaskManager();

        static BPRegulatorRuntimeService()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["BusinessProcess_MaxWorkflowsPerServiceInstance"], out s_maxWorkflowsPerServiceInstance))
                s_maxWorkflowsPerServiceInstance = 50;
            if (!int.TryParse(ConfigurationManager.AppSettings["BusinessProcess_MoreAssignableItemsToMaxConcurrentPerBPDefinition"], out s_moreAssignableItemsToMaxConcurrentPerBPDefinition))
                s_moreAssignableItemsToMaxConcurrentPerBPDefinition = 0;
            if (!int.TryParse(ConfigurationManager.AppSettings["BusinessProcess_ArchiveBeforeNbOfDays"], out s_archiveBeforeNbOfDays))
                s_archiveBeforeNbOfDays = 5;
            if (!int.TryParse(ConfigurationManager.AppSettings["BusinessProcess_NbOfInstancesToArchivePerQuery"], out s_nbOfInstancesToArchivePerQuery))
                s_nbOfInstancesToArchivePerQuery = 100;
        }

        public override void Execute()
        {
            TransactionLocker.Instance.TryLock("BPRegulatorRuntimeService_Execute", () =>
            {
                var bpServiceInstances = GetRunningServiceInstances();
                if (bpServiceInstances == null || bpServiceInstances.Count == 0)
                    return;
                AssignPendingInstancesToServices(bpServiceInstances);
                _bpInstanceManager.ArchiveInstances(BPInstanceStatusAttribute.GetClosedStatuses(), DateTime.Today.AddDays(-s_archiveBeforeNbOfDays), s_nbOfInstancesToArchivePerQuery);

                TryTriggerSubscribedBPs();
            });
        }

        private void TryTriggerSubscribedBPs()
        {
            BPTimeSubscriptionManager timeSubscriptionManager = new BPTimeSubscriptionManager();
            BPEventManager eventManager = new BPEventManager();

            IEnumerable<BPTimeSubscription> bpTimeSubscriptions = timeSubscriptionManager.GetDueBPTimeSubscriptions();
            if (bpTimeSubscriptions == null || bpTimeSubscriptions.Count() == 0)
                return;

            foreach (BPTimeSubscription bpTimeSubscription in bpTimeSubscriptions)
            {
                eventManager.TriggerProcessEvent(new TriggerProcessEventInput() { BookmarkName = bpTimeSubscription.Bookmark, ProcessInstanceId = bpTimeSubscription.ProcessInstanceId, EventData = bpTimeSubscription.Payload });
                timeSubscriptionManager.DeleteBPTimeSubscription(bpTimeSubscription.BPTimeSubscriptionId);
            }
        }

        private List<Runtime.Entities.RuntimeServiceInstance> GetRunningServiceInstances()
        {
            return _serviceInstanceManager.GetServices(BusinessProcessService.SERVICE_TYPE_UNIQUE_NAME);
        }

        static List<BPInstanceStatus> s_pendingStatuses = BPInstanceStatusAttribute.GetNonClosedStatuses();

        private void AssignPendingInstancesToServices(List<RuntimeServiceInstance> bpServiceInstances)
        {
            Dictionary<Guid, StructuredProcessSynchronisation> structuredProcessSynchronisations = _processSynchronisationManager.GetStructuredProcessSynchronisations();

            var pendingInstances = _bpInstanceManager.GetPendingInstancesInfo(s_pendingStatuses);
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

                HashSet<Guid> startedBPInstanceDefinitionIds = new HashSet<Guid>();
                HashSet<Guid> startedBPInstanceTaskIds = new HashSet<Guid>();
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
                            AddStartedBPInstance(startedBPInstances, pendingInstanceInfo, startedBPInstanceDefinitionIds, startedBPInstanceTaskIds);
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
                                AddStartedBPInstance(startedBPInstances, pendingInstanceInfo, startedBPInstanceDefinitionIds, startedBPInstanceTaskIds);
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

                    string allowInstanceToRunOutputMessage = null;
                    if (definitionExtendedSettings.CanRunBPInstance(canRunBPInstanceContext) && AllowInstanceToRun(pendingInstanceInfo, startedBPInstanceDefinitionIds, startedBPInstanceTaskIds, structuredProcessSynchronisations, out allowInstanceToRunOutputMessage))
                    {
                        if (TryAssignLeastBusyServiceToBPInstance(pendingInstanceInfo, servicesToAssign, pendingInstancesToUpdate))
                            AddStartedBPInstance(startedBPInstances, pendingInstanceInfo, startedBPInstanceDefinitionIds, startedBPInstanceTaskIds);
                    }
                    else
                    {
                        string message = canRunBPInstanceContext.Reason != null ? canRunBPInstanceContext.Reason : allowInstanceToRunOutputMessage;
                        if (message != null && message != pendingInstanceInfo.LastMessage)
                        {
                            BPTrackingChannel.Current.WriteTrackingMessage(new BPTrackingMessage
                            {
                                ProcessInstanceId = pendingInstanceInfo.ProcessInstanceID,
                                ParentProcessId = pendingInstanceInfo.ParentProcessID,
                                Severity = Vanrise.Entities.LogEntryType.Warning,
                                TrackingMessage = message,
                                EventTime = DateTime.Now
                            });
                            _bpInstanceManager.UpdateInstanceLastMessage(pendingInstanceInfo.ProcessInstanceID, message);
                        }
                        if (pendingInstanceInfo.Status != BPInstanceStatus.Postponed)
                        {
                            pendingInstanceInfo.Status = BPInstanceStatus.Postponed;
                            BPDefinitionInitiator.UpdateProcessStatus(pendingInstanceInfo.ProcessInstanceID, pendingInstanceInfo.ParentProcessID, pendingInstanceInfo.Status, BPInstanceAssignmentStatus.Free, null, null);
                        }
                    }
                }

                if (pendingInstancesToUpdate.Count > 0)
                    _bpInstanceManager.UpdateServiceInstancesAndAssignmentStatus(pendingInstancesToUpdate);

                if (instanceCancellationRequests.Count > 0)
                    SendCancellationRequests(serviceInstancesInfo, instanceCancellationRequests);
                NotifyServiceInstances(serviceInstancesInfo);

                //delete BPEvents for BPInstances that are not running
                if (processInstanceIdsHavingEvents.Count > 0)
                {
                    foreach (var processInstanceId in processInstanceIdsHavingEvents)
                    {
                        BPInstance processInstance;
                        if (!pendingInstancesDict.TryGetValue(processInstanceId, out processInstance) || processInstance.Status == BPInstanceStatus.Suspended || processInstance.Status == BPInstanceStatus.Cancelled)
                            _bpEventDataManager.DeleteProcessInstanceEvents(processInstanceId);
                    }
                }
            }
        }

        private void AddStartedBPInstance(List<BPInstance> startedBPInstances, BPInstance pendingInstanceInfo, HashSet<Guid> startedBPInstanceDefinitionIds, HashSet<Guid> startedBPInstanceTaskIds)
        {
            startedBPInstances.Add(pendingInstanceInfo);
            startedBPInstanceDefinitionIds.Add(pendingInstanceInfo.DefinitionID);
            if (pendingInstanceInfo.TaskId.HasValue)
                startedBPInstanceTaskIds.Add(pendingInstanceInfo.TaskId.Value);
        }

        private bool AllowInstanceToRun(BPInstance pendingInstanceInfo, HashSet<Guid> startedBPInstanceDefinitionIds, HashSet<Guid> startedBPInstanceTaskIds,
            Dictionary<Guid, StructuredProcessSynchronisation> structuredProcessSynchronisations, out string message)
        {
            message = null;

            var structuredProcessSynchronisation = structuredProcessSynchronisations != null ? structuredProcessSynchronisations.GetRecord(pendingInstanceInfo.DefinitionID) : null;
            if (structuredProcessSynchronisation == null)
                return true;

            bool isValid = true;

            if (structuredProcessSynchronisation.LinkedProcessSynchronisationItems != null)
            {
                var linkedProcessSynchronisationItems = structuredProcessSynchronisation.LinkedProcessSynchronisationItems;
                if (!IsValidToRun(pendingInstanceInfo, linkedProcessSynchronisationItems, startedBPInstanceDefinitionIds, startedBPInstanceTaskIds, ref message))
                    isValid = false;
            }

            if (pendingInstanceInfo.TaskId.HasValue && structuredProcessSynchronisation.LinkedProcessSynchronisationItemsByTaskId != null)
            {
                var taskLinkedProcessSynchronisationItems = structuredProcessSynchronisation.LinkedProcessSynchronisationItemsByTaskId.GetRecord(pendingInstanceInfo.TaskId.Value);
                if (taskLinkedProcessSynchronisationItems != null)
                {
                    if (!IsValidToRun(pendingInstanceInfo, taskLinkedProcessSynchronisationItems, startedBPInstanceDefinitionIds, startedBPInstanceTaskIds, ref message))
                        isValid = false;
                }
            }

            return isValid;
        }

        private bool IsValidToRun(BPInstance pendingInstanceInfo, LinkedProcessSynchronisationItems linkedProcessSynchronisationItems, HashSet<Guid> startedBPInstanceDefinitionIds,
            HashSet<Guid> startedBPInstanceTaskIds, ref string message)
        {
            if (linkedProcessSynchronisationItems.TaskIds != null && linkedProcessSynchronisationItems.TaskIds.Count > 0)
            {
                foreach (Guid taskId in linkedProcessSynchronisationItems.TaskIds)
                {
                    if (startedBPInstanceTaskIds.Contains(taskId))
                    {
                        if (string.IsNullOrEmpty(message))
                            message = string.Format("Waiting Scheduler Service '{0}'", _schedulerTaskManager.GetTask(taskId).Name);

                        return false;
                    }
                }
            }

            if (linkedProcessSynchronisationItems.BPDefinitionIds != null && linkedProcessSynchronisationItems.BPDefinitionIds.Count > 0)
            {
                foreach (Guid bpDefinitionId in linkedProcessSynchronisationItems.BPDefinitionIds)
                {
                    if (startedBPInstanceDefinitionIds.Contains(bpDefinitionId))
                    {
                        if (string.IsNullOrEmpty(message))
                            message = string.Format("Waiting Business Process '{0}'", _bpDefinitionManager.GetBPDefinition(bpDefinitionId).Title);

                        return false;
                    }
                }
            }

            bool areHoldRequestsReady = true;
            if (linkedProcessSynchronisationItems.ExecutionFlowDefinitionIds != null && linkedProcessSynchronisationItems.ExecutionFlowDefinitionIds.Count > 0)
            {
                DateTimeRange dateTimeRange = _holdRequestManager.GetDBDateTimeRange();

                foreach (Guid executionFlowDefinitionId in linkedProcessSynchronisationItems.ExecutionFlowDefinitionIds)
                {
                    HoldRequest existingHoldRequest;
                    Dictionary<Guid, HoldRequest> existingHoldRequests = _holdRequestManager.GetHoldRequestsExecutionFlowDefinition(pendingInstanceInfo.ProcessInstanceID);
                    if (existingHoldRequests == null || existingHoldRequests.Count == 0 || !existingHoldRequests.TryGetValue(executionFlowDefinitionId, out existingHoldRequest))
                    {
                        var stageNames = _queueExecutionFlowDefinitionManager.GetFlowStagesName(executionFlowDefinitionId);
                        _holdRequestManager.InsertHoldRequest(pendingInstanceInfo.ProcessInstanceID, executionFlowDefinitionId, dateTimeRange.From, dateTimeRange.To, stageNames, null, HoldRequestStatus.Pending);
                        areHoldRequestsReady = false;

                        if (string.IsNullOrEmpty(message))
                            message = string.Format("Waiting Execution Flow Definition '{0}'", _queueExecutionFlowDefinitionManager.GetExecutionFlowDefinitionTitle(executionFlowDefinitionId));

                    }
                    else if (existingHoldRequest.Status != HoldRequestStatus.CanBeStarted)
                    {
                        areHoldRequestsReady = false;

                        if (string.IsNullOrEmpty(message))
                            message = string.Format("Waiting Execution Flow Definition '{0}'", _queueExecutionFlowDefinitionManager.GetExecutionFlowDefinitionTitle(executionFlowDefinitionId));
                    }
                }
            }

            if (!areHoldRequestsReady)
                return false;

            return true;
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
                    if (pendingInstanceInfo.ServiceInstanceId.HasValue)
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
