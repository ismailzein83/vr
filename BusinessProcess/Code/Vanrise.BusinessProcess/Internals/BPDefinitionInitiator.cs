using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Configuration;
using System.Activities.DurableInstancing;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Data;
using Vanrise.Common;
using Vanrise.Runtime;
using Vanrise.BusinessProcess.Business;
using Vanrise.Entities;
using Vanrise.Security.Entities;
using Vanrise.Security.Business;
using System.Runtime.DurableInstancing;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.IO;

namespace Vanrise.BusinessProcess
{
    internal class BPDefinitionInitiator
    {
        #region Members/Ctor

        static bool s_AddConsoleTracking = ConfigurationManager.AppSettings["AddBusinessProcessConsoleTracking"] == "true";
        static bool s_AddActivityEventsTracking = ConfigurationManager.AppSettings["BusinessProcess_ActivityEventsTracking"] == "true";
        internal static int s_defaultMaxConcurrentWorkflowsPerDefinition;

        static BPDefinitionInitiator()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["BusinessProcess_DefaultMaxConcurrentWorkflowsPerDefinition"], out s_defaultMaxConcurrentWorkflowsPerDefinition))
                s_defaultMaxConcurrentWorkflowsPerDefinition = 10;
        }

        Guid _serviceInstanceId;
        BPDefinition _definition;

        static IBPInstanceDataManager s_instanceDataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
        static IBPEventDataManager s_eventDataManager = BPDataManagerFactory.GetDataManager<IBPEventDataManager>();
        static IBPTaskDataManager s_taskDataManager = BPDataManagerFactory.GetDataManager<IBPTaskDataManager>();
        static IBPInstancePersistenceDataManager s_instancePersistenceDataManager = BPDataManagerFactory.GetDataManager<IBPInstancePersistenceDataManager>();

        Activity _workflowDefinition;

        private ConcurrentDictionary<long, BPRunningInstance> _runningInstances = new ConcurrentDictionary<long, BPRunningInstance>();

        BPDefinitionManager _definitionManager = new BPDefinitionManager();
        Guid _definitionId;
        int _maxNbOfThreads;

        static IEnumerable<BPInstanceStatus> s_acceptableBPStatusesToRun = new BPInstanceStatus[] { BPInstanceStatus.New, BPInstanceStatus.Postponed, BPInstanceStatus.Waiting, BPInstanceStatus.Running };
        public BPDefinitionInitiator(Guid serviceInstanceId, BPDefinition definition)
        {
            _serviceInstanceId = serviceInstanceId;
            definition.WorkflowType.ThrowIfNull("bpDefinition.WorkflowType", definition.BPDefinitionID);
            _definition = definition;
            _definitionId = definition.BPDefinitionID;
            try
            {
                _workflowDefinition = Activator.CreateInstance(definition.WorkflowType) as Activity;
            }
            catch (Exception ex)
            {
                Vanrise.Common.LoggerFactory.GetLogger().WriteError("Unable to create instance from type '{0}' for BPDefinitionID: '{1}'. Error: {2}", definition.WorkflowType, definition.BPDefinitionID, ex.ToString());
                throw;
            }
            if (_workflowDefinition == null)
                throw new Exception(String.Format("'{0}' is not of type Activity", definition.WorkflowType));
            _maxNbOfThreads = _definition.Configuration != null && _definition.Configuration.MaxConcurrentWorkflows.HasValue ? _definition.Configuration.MaxConcurrentWorkflows.Value : s_defaultMaxConcurrentWorkflowsPerDefinition;
        }

        #endregion

        #region Workflow Execution

        bool _isRunning;

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
                _definition = _definitionManager.GetBPDefinition(_definitionId);
                int nbOfRunningInstances = GetNbOfRunningInstances();
                if (nbOfRunningInstances >= _maxNbOfThreads)
                    return;
                int nbOfThreads = _maxNbOfThreads - nbOfRunningInstances;
                List<BPInstance> pendingInstances = s_instanceDataManager.GetPendingInstances(_definitionId, s_acceptableBPStatusesToRun, BPInstanceAssignmentStatus.AssignedForExecution, nbOfThreads, _serviceInstanceId);
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

        private int GetNbOfRunningInstances()
        {
            return _runningInstances.Where(itm => itm.Value.BPInstance.Status == BPInstanceStatus.Running && !itm.Value.IsIdle).Count();
        }
              

        #endregion

        #region Private Methods

        private void RunProcess(BPInstance bpInstance)
        {
            IDictionary<string, object> inputs = null;
            if (bpInstance.InputArgument != null)
            {
                inputs = new Dictionary<string, object>();
                inputs.Add("Input", bpInstance.InputArgument);
            }
            
            WorkflowApplication wfApp = inputs != null ? new WorkflowApplication(_workflowDefinition, inputs) : new WorkflowApplication(_workflowDefinition);
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
                        WFApplication = new WorkflowApplication(_workflowDefinition)
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
                        s_instanceDataManager.UpdateInstanceAssignmentStatus(bpInstance.ProcessInstanceID, BPInstanceAssignmentStatus.Executing);
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
                if (_definitionManager.GetBPDefinitionExtendedSettings(_definition).ShouldPersist(new BPDefinitionShouldPersistContext { BPInstance = runningInstance.BPInstance }))
                {
                    BPRunningInstance dummy;
                    _runningInstances.TryRemove(runningInstance.BPInstance.ProcessInstanceID, out dummy);
                    return PersistableIdleAction.Unload;
                }
                else
                {
                    s_instanceDataManager.UpdateInstanceAssignmentStatus(runningInstance.BPInstance.ProcessInstanceID, BPInstanceAssignmentStatus.Free);
                    runningInstance.IsIdle = true;
                    return PersistableIdleAction.None;
                }
            };
        }

        void OnWorkflowCompleted(BPInstance bpInstance, WorkflowApplicationCompletedEventArgs e)
        {
            BPRunningInstance dummy;
            _runningInstances.TryRemove(bpInstance.ProcessInstanceID, out dummy);
            string logEventType = bpInstance.InputArgument.GetDefinitionTitle();
            string processTitle = bpInstance.Title;
            Exception terminationException = null;
            bpInstance.AssignmentStatus = BPInstanceAssignmentStatus.Free;
            if (e.CompletionState == ActivityInstanceState.Closed)
            {
                bpInstance.Status = BPInstanceStatus.Completed;
                UpdateProcessStatus(bpInstance, false);
                LoggerFactory.GetLogger().WriteEntry(logEventType, GetGeneralLogViewRequiredPermissionSetId(bpInstance), LogEntryType.Information, "Process '{0}' completed", processTitle);
            }
            else
            {
                terminationException = Utilities.WrapException(e.TerminationException, String.Format("Process '{0}' failed", processTitle));
                BPTrackingChannel.Current.WriteException(bpInstance.ProcessInstanceID, bpInstance.ParentProcessID, terminationException);
                bpInstance.LastMessage = String.Format("Workflow Finished Unsuccessfully. Status: {0}. Error: {1}", e.CompletionState, e.TerminationException);
                bpInstance.Status = BPInstanceStatus.Aborted;
                UpdateProcessStatus(bpInstance, false);

                LoggerFactory.GetExceptionLogger().WriteException(logEventType, GetGeneralLogViewRequiredPermissionSetId(bpInstance), terminationException);
                Console.WriteLine("{0}: {1}", DateTime.Now, bpInstance.LastMessage);
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
            s_instanceDataManager.UpdateInstanceStatus(processInstanceId, status, assignmentStatus, errorMessage, clearServiceInstanceId, workflowInstanceId);
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
