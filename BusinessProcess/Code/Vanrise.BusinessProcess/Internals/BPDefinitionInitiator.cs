using System;
using System.Activities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.BusinessProcess
{
    internal class BPDefinitionInitiator
    {
        #region Members/Ctor

        static bool s_AddConsoleTracking = ConfigurationManager.AppSettings["AddBusinessProcessConsoleTracking"] == "true";
        static bool s_AddActivityEventsTracking = ConfigurationManager.AppSettings["BusinessProcess_ActivityEventsTracking"] == "true";
        internal static int s_defaultMaxConcurrentWorkflowsPerDefinition;

        static IBPEventDataManager s_eventDataManager = BPDataManagerFactory.GetDataManager<IBPEventDataManager>();
        static IBPTaskDataManager s_taskDataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();
        static IBPInstancePersistenceDataManager s_instancePersistenceDataManager = BPDataManagerFactory.GetDataManager<IBPInstancePersistenceDataManager>();

        static BPDefinitionManager s_definitionManager = new BPDefinitionManager();
        static BPInstanceManager s_bpInstanceManager = new BPInstanceManager();
        static VRWorkflowManager s_vrWorkflowManager = new VRWorkflowManager();

        Guid _definitionId;
        int _maxNbOfThreads;
        Guid _serviceInstanceId;
        BPDefinition _definition;
        
        Activity _workflowDefinition;
        private Activity WorkflowDefinition
        {
            get
            {
                if (_workflowDefinition != null)
                    return _workflowDefinition;
                else
                    return s_vrWorkflowManager.GetVRWorkflowActivity(_definition.VRWorkflowId.Value);
            }
        }

        private ConcurrentDictionary<long, BPRunningInstance> _runningInstances = new ConcurrentDictionary<long, BPRunningInstance>();

        static BPDefinitionInitiator()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["BusinessProcess_DefaultMaxConcurrentWorkflowsPerDefinition"], out s_defaultMaxConcurrentWorkflowsPerDefinition))
                s_defaultMaxConcurrentWorkflowsPerDefinition = 10;
        }

        public BPDefinitionInitiator(Guid serviceInstanceId, BPDefinition definition)
        {
            _serviceInstanceId = serviceInstanceId;

            if (definition.WorkflowType == null && !definition.VRWorkflowId.HasValue)
                throw new VRBusinessException(string.Format("WorkflowType or VRWorkflowId should have value for BPDefinitionID: '{0}'", definition.BPDefinitionID));

            _definition = definition;
            _definitionId = definition.BPDefinitionID;
            try
            {
                if (definition.WorkflowType != null)
                {
                    _workflowDefinition = Activator.CreateInstance(definition.WorkflowType) as Activity;
                    if (_workflowDefinition == null)
                        throw new Exception(String.Format("'{0}' is not of type Activity", definition.WorkflowType));
                }
            }
            catch (Exception ex)
            {
                Vanrise.Common.LoggerFactory.GetLogger().WriteError("Unable to create instance from type '{0}' for BPDefinitionID: '{1}'. Error: {2}", definition.WorkflowType, definition.BPDefinitionID, ex.ToString());
                throw;
            }

            _maxNbOfThreads = _definition.Configuration != null && _definition.Configuration.MaxConcurrentWorkflows.HasValue ? _definition.Configuration.MaxConcurrentWorkflows.Value : s_defaultMaxConcurrentWorkflowsPerDefinition;
        }

        #endregion

        #region Workflow Execution

        bool _isRunning;
        static IEnumerable<BPInstanceStatus> s_acceptableBPStatusesToRun = BPInstanceStatusAttribute.GetNonClosedStatuses();
        internal void RunPendingProcesses()
        {
            lock (this)
            {
                if (_isRunning)
                    return;
                _isRunning = true;
            }
            try
            {
                _definition = s_definitionManager.GetBPDefinition(_definitionId);
                int nbOfRunningInstances = GetNbOfRunningInstances();
                if (nbOfRunningInstances >= _maxNbOfThreads)
                    return;
                int nbOfThreads = _maxNbOfThreads - nbOfRunningInstances;
                List<BPInstance> pendingInstances = s_bpInstanceManager.GetPendingInstances(_definitionId, s_acceptableBPStatusesToRun, BPInstanceAssignmentStatus.AssignedForExecution, nbOfThreads, _serviceInstanceId);
                if (pendingInstances != null)
                {
                    foreach (var instance in pendingInstances)
                    {
                        if (instance.Status == BPInstanceStatus.New || instance.Status == BPInstanceStatus.Postponed)
                            RunProcess(instance);
                        else
                            ResumeProcess(instance);
                    }
                }
            }
            finally
            {
                lock (this)
                {
                    _isRunning = false;
                }
            }
        }

        internal void ExecuteCancellationRequest(long bpInstanceId, string reason)
        {
            BPRunningInstance runningInstance;
            if (_runningInstances.TryGetValue(bpInstanceId, out runningInstance))
            {
                lock (runningInstance)//lock runningInstance to make sure the process is not being completed while applying cancellation
                {
                    if (!runningInstance.IsWorkflowCompleted)
                    {
                        BPTrackingChannel.Current.WriteTrackingMessage(new BPTrackingMessage
                        {
                            ProcessInstanceId = runningInstance.BPInstance.ProcessInstanceID,
                            ParentProcessId = runningInstance.BPInstance.ParentProcessID,
                            TrackingMessage = reason,
                            Severity = LogEntryType.Warning,
                            EventTime = DateTime.Now
                        });
                        runningInstance.BPInstance.Status = BPInstanceStatus.Cancelling;
                        runningInstance.BPInstance.LastMessage = "Process is cancelled";
                        UpdateProcessStatus(runningInstance.BPInstance, false);
                        runningInstance.WFApplication.Cancel();
                    }
                }
            }
            else
            {
                BPInstance bpInstance = s_bpInstanceManager.GetBPInstance(bpInstanceId, false);
                if (bpInstance != null && !BPInstanceStatusAttribute.GetAttribute(bpInstance.Status).IsClosed)
                {
                    BPTrackingChannel.Current.WriteTrackingMessage(new BPTrackingMessage
                    {
                        ProcessInstanceId = bpInstance.ProcessInstanceID,
                        ParentProcessId = bpInstance.ParentProcessID,
                        TrackingMessage = reason,
                        Severity = LogEntryType.Warning,
                        EventTime = DateTime.Now
                    });
                    bpInstance.Status = BPInstanceStatus.Cancelled;
                    bpInstance.LastMessage = "Process is cancelled";
                    UpdateProcessStatus(bpInstance, false);
                    string logEventType = bpInstance.InputArgument.GetDefinitionTitle();
                    string processTitle = bpInstance.Title;
                    LoggerFactory.GetLogger().WriteEntry(logEventType, GetGeneralLogViewRequiredPermissionSetId(bpInstance), LogEntryType.Error, "Process '{0}' cancelled", processTitle);
                }
            }
        }

        private int GetNbOfRunningInstances()
        {
            return _runningInstances.Where(itm => !itm.Value.IsIdle).Count();
        }

        #endregion

        #region Private Methods

        private void RunProcess(BPInstance bpInstance)
        {
            IDictionary<string, object> inputs = null;
            if (!_definition.VRWorkflowId.HasValue)
            {
                if (bpInstance.InputArgument != null)
                {
                    inputs = new Dictionary<string, object>();
                    inputs.Add("Input", bpInstance.InputArgument);
                }
            }
            else
            {
                VRWorkflowInputArgument inputArgument = bpInstance.InputArgument.CastWithValidate<VRWorkflowInputArgument>("bpInstance.InputArgument", bpInstance.ProcessInstanceID);
                inputs = inputArgument.Arguments;
            }

            WorkflowApplication wfApp = inputs != null ? new WorkflowApplication(WorkflowDefinition, inputs) : new WorkflowApplication(WorkflowDefinition);
            bpInstance.WorkflowInstanceID = wfApp.Id;
            BPRunningInstance runningInstance = new BPRunningInstance
            {
                BPInstance = bpInstance,
                WFApplication = wfApp
            };
            bpInstance.InitiatorUserId = bpInstance.InputArgument.UserId;
            _runningInstances.TryAdd(bpInstance.ProcessInstanceID, runningInstance);

            PrepareWFAppForExecution(runningInstance);

            bpInstance.Status = BPInstanceStatus.Running;
            bpInstance.AssignmentStatus = BPInstanceAssignmentStatus.Executing;
            UpdateProcessStatus(bpInstance, false);
            wfApp.Run();
            string logEventType = bpInstance.InputArgument.GetDefinitionTitle();
            string processTitle = bpInstance.Title;
            LoggerFactory.GetLogger().WriteEntry(logEventType, GetGeneralLogViewRequiredPermissionSetId(bpInstance), LogEntryType.Information, "Process '{0}' started", processTitle);
        }

        private void ResumeProcess(BPInstance bpInstance)
        {
            var bpEvents = s_eventDataManager.GetInstancesEvents(new List<long> { bpInstance.ProcessInstanceID });
            if (bpEvents != null && bpEvents.Count() > 0)
            {
                BPRunningInstance runningInstance;
                if (!_runningInstances.TryGetValue(bpInstance.ProcessInstanceID, out runningInstance))
                {
                    runningInstance = new BPRunningInstance
                    {
                        BPInstance = bpInstance,
                        WFApplication = new WorkflowApplication(WorkflowDefinition)
                    };
                    PrepareWFAppForExecution(runningInstance);
                    if (!_runningInstances.TryAdd(bpInstance.ProcessInstanceID, runningInstance))
                        throw new Exception(String.Format("_runningInstances.TryAdd failed for ProcessInstanceID '{0}'", bpInstance.ProcessInstanceID));
                    runningInstance.WFApplication.Load(bpInstance.WorkflowInstanceID.Value);
                    bpInstance.Status = BPInstanceStatus.Running;
                    bpInstance.AssignmentStatus = BPInstanceAssignmentStatus.Executing;
                    UpdateProcessStatus(bpInstance, false);
                }
                else
                {
                    if (bpInstance.AssignmentStatus != BPInstanceAssignmentStatus.Executing)
                    {
                        s_bpInstanceManager.UpdateInstanceAssignmentStatus(bpInstance.ProcessInstanceID, BPInstanceAssignmentStatus.Executing);
                        bpInstance.AssignmentStatus = BPInstanceAssignmentStatus.Executing;
                    }
                    runningInstance.IsIdle = false;
                }
                foreach (var evnt in bpEvents)
                {
                    TriggerWFEvent(runningInstance, evnt);
                }
            }
        }

        private void PrepareWFAppForExecution(BPRunningInstance runningInstance)
        {
            runningInstance.WFApplication.InstanceStore = new BPInstanceStore(runningInstance.BPInstance);
            var sharedInstanceData = new BPSharedInstanceData(runningInstance.BPInstance, _definition);
            runningInstance.WFApplication.Extensions.Add(sharedInstanceData);

            if (s_AddConsoleTracking)
                runningInstance.WFApplication.Extensions.Add(ConsoleTracking.Instance);
            if (s_AddActivityEventsTracking)
                runningInstance.WFApplication.Extensions.Add(new ActivityEventsTracking(runningInstance.BPInstance));

            runningInstance.WFApplication.Completed = (e) =>
            {
                sharedInstanceData.ClearCacheManagers();
                OnWorkflowCompleted(runningInstance.BPInstance, e);
            };
            runningInstance.WFApplication.PersistableIdle = delegate(WorkflowApplicationIdleEventArgs e)
            {
                if (s_definitionManager.GetBPDefinitionExtendedSettings(_definition).ShouldPersist(new BPDefinitionShouldPersistContext { BPInstance = runningInstance.BPInstance }))
                {
                    BPRunningInstance dummy;
                    _runningInstances.TryRemove(runningInstance.BPInstance.ProcessInstanceID, out dummy);
                    return PersistableIdleAction.Unload;
                }
                else
                {
                    s_bpInstanceManager.UpdateInstanceAssignmentStatus(runningInstance.BPInstance.ProcessInstanceID, BPInstanceAssignmentStatus.Free);
                    runningInstance.IsIdle = true;
                    return PersistableIdleAction.None;
                }
            };
        }

        void OnWorkflowCompleted(BPInstance bpInstance, WorkflowApplicationCompletedEventArgs e)
        {
            BPRunningInstance runningInstance;
            _runningInstances.TryRemove(bpInstance.ProcessInstanceID, out runningInstance);
            runningInstance.ThrowIfNull("runningInstance", bpInstance.ProcessInstanceID);
            lock (runningInstance)
            {
                runningInstance.IsWorkflowCompleted = true;
            }

            string logEventType = bpInstance.InputArgument.GetDefinitionTitle();
            string processTitle = bpInstance.Title;
            Exception terminationException = null;
            bpInstance.AssignmentStatus = BPInstanceAssignmentStatus.Free;
            if (e.CompletionState == ActivityInstanceState.Closed && bpInstance.Status != BPInstanceStatus.Cancelling)
            {
                bpInstance.Status = BPInstanceStatus.Completed;
                UpdateProcessStatus(bpInstance, false);
                LoggerFactory.GetLogger().WriteEntry(logEventType, GetGeneralLogViewRequiredPermissionSetId(bpInstance), LogEntryType.Information, "Process '{0}' completed", processTitle);
            }
            else
            {
                if (bpInstance.Status == BPInstanceStatus.Cancelling)
                {
                    bpInstance.Status = BPInstanceStatus.Cancelled;
                    LoggerFactory.GetLogger().WriteEntry(logEventType, GetGeneralLogViewRequiredPermissionSetId(bpInstance), LogEntryType.Error, "Process '{0}' cancelled", processTitle);
                }
                else
                {
                    bpInstance.Status = BPInstanceStatus.Aborted;
                    terminationException = Utilities.WrapException(e.TerminationException, String.Format("Process '{0}' failed", processTitle));
                    BPTrackingChannel.Current.WriteException(bpInstance.ProcessInstanceID, bpInstance.ParentProcessID, terminationException);
                    bpInstance.LastMessage = String.Format("Workflow Finished Unsuccessfully. Status: {0}. Error: {1}", e.CompletionState, e.TerminationException);
                    LoggerFactory.GetExceptionLogger().WriteException(logEventType, GetGeneralLogViewRequiredPermissionSetId(bpInstance), terminationException);
                }
                UpdateProcessStatus(bpInstance, false);
            }

            object processOutput = null;
            if (e.Outputs != null)
                e.Outputs.TryGetValue("Output", out processOutput);

            if (bpInstance.ParentProcessID.HasValue)
                NotifyParentBPChildCompleted(bpInstance.ProcessInstanceID, bpInstance.ParentProcessID.Value, bpInstance.Status, bpInstance.LastMessage, terminationException, processOutput);

            NotifyBPCompleted(_definition, bpInstance, processOutput);

            RunPendingProcesses();
            GC.Collect();
        }

        static RequiredPermissionSetManager s_requiredPermissionSetManager = new RequiredPermissionSetManager();
        internal static int? GetGeneralLogViewRequiredPermissionSetId(BPInstance bpInstance)
        {
            if (bpInstance.ViewRequiredPermissionSetId.HasValue)
            {
                RequiredPermissionSet viewRequiredPermissionSet = s_requiredPermissionSetManager.GetRequiredPermissionSet(bpInstance.ViewRequiredPermissionSetId.Value);
                viewRequiredPermissionSet.ThrowIfNull("viewRequiredPermissionSet", bpInstance.ViewRequiredPermissionSetId.Value);
                return s_requiredPermissionSetManager.GetRequiredPermissionSetId(LoggerFactory.LOGGING_REQUIREDPERMISSIONSET_MODULENAME, viewRequiredPermissionSet.RequiredPermissionString);
            }
            else
            {
                return null;
            }
        }

        internal static void NotifyParentBPChildCompleted(long bpInstanceId, long parentBPInstanecId, BPInstanceStatus status, string errorMessage, Exception exception, object processOutput)
        {
            var eventData = new ProcessCompletedEventPayload
            {
                ProcessStatus = status,
                LastProcessMessage = errorMessage,
                ErrorBusinessMessage = exception != null ? Utilities.GetExceptionBusinessMessage(exception) : errorMessage,
                ExceptionDetail = exception != null ? exception.ToString() : null,
                ProcessOutput = processOutput
            };
            s_eventDataManager.InsertEvent(parentBPInstanecId, bpInstanceId.ToString(), eventData);
        }

        internal static void NotifyBPCompleted(BPInstance bpInstance, object processOutput)
        {
            NotifyBPCompleted(new BPDefinitionManager().GetBPDefinition(bpInstance.DefinitionID), bpInstance, processOutput);
        }

        internal static void NotifyBPCompleted(BPDefinition bpDefinition, BPInstance bpInstance, object processOutput)
        {
            if (bpInstance.Status != BPInstanceStatus.Completed)
            {
                s_taskDataManager.CancelNotCompletedTasks(bpInstance.ProcessInstanceID);
                s_instancePersistenceDataManager.DeleteState(bpInstance.ProcessInstanceID);
            }
            BPDefinitionBPExecutionCompletedContext context = new BPDefinitionBPExecutionCompletedContext() { BPInstance = bpInstance };
            new BPDefinitionManager().GetBPDefinitionExtendedSettings(bpDefinition).OnBPExecutionCompleted(context);
            if (bpInstance.CompletionNotifier != null)
            {
                var eventData = new Entities.ProcessCompletedEventPayload
                {
                    ProcessStatus = bpInstance.Status,
                    LastProcessMessage = bpInstance.LastMessage,
                    ProcessOutput = processOutput
                };
                bpInstance.CompletionNotifier.OnProcessInstanceCompleted(eventData);
            }
        }

        void TriggerWFEvent(BPRunningInstance runningInstance, BPEvent bpEvent)
        {
            runningInstance.WFApplication.ResumeBookmark(bpEvent.Bookmark, bpEvent.Payload);
            s_eventDataManager.DeleteEvent(bpEvent.BPEventID);
            BPTrackingChannel.Current.WriteTrackingMessage(new BPTrackingMessage
            {
                ProcessInstanceId = runningInstance.BPInstance.ProcessInstanceID,
                ParentProcessId = runningInstance.BPInstance.ParentProcessID,
                TrackingMessage = String.Format("Event '{0}' triggerred", bpEvent.Bookmark),
                Severity = LogEntryType.Verbose,
                EventTime = DateTime.Now
            });
        }

        internal static void UpdateProcessStatus(BPInstance bpInstance, bool clearServiceInstanceId)
        {
            UpdateProcessStatus(bpInstance.ProcessInstanceID, bpInstance.ParentProcessID, bpInstance.Status, bpInstance.AssignmentStatus, bpInstance.LastMessage, clearServiceInstanceId, bpInstance.WorkflowInstanceID);
        }

        internal static void UpdateProcessStatus(long processInstanceId, long? parentProcessId, BPInstanceStatus status, BPInstanceAssignmentStatus assignmentStatus, string errorMessage, Guid? workflowInstanceId = null)
        {
            UpdateProcessStatus(processInstanceId, parentProcessId, status, assignmentStatus, errorMessage, false, workflowInstanceId);
        }

        internal static void UpdateProcessStatus(long processInstanceId, long? parentProcessId, BPInstanceStatus status, BPInstanceAssignmentStatus assignmentStatus, string errorMessage, bool clearServiceInstanceId, Guid? workflowInstanceId)
        {
            s_bpInstanceManager.UpdateInstanceStatus(processInstanceId, status, assignmentStatus, errorMessage, clearServiceInstanceId, workflowInstanceId);
            LogEntryType statusChangedTrackingSeverity = BPInstanceStatusAttribute.GetAttribute(status).TrackingSeverity;
            BPTrackingChannel.Current.WriteTrackingMessage(new BPTrackingMessage
            {
                ProcessInstanceId = processInstanceId,
                ParentProcessId = parentProcessId,
                TrackingMessage = String.Format("Status changed to '{0}'", status),
                Severity = statusChangedTrackingSeverity,
                EventTime = DateTime.Now
            });
        }

        #endregion

        #region Private Classes

        public class BPDefinitionShouldPersistContext : IBPDefinitionShouldPersistContext
        {
            public BPInstance BPInstance
            {
                get;
                set;
            }
        }

        #endregion
    }
}
