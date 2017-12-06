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

namespace Vanrise.BusinessProcess
{
    internal class BPDefinitionInitiator
    {
        #region Members/Ctor

        static bool s_AddConsoleTracking = ConfigurationManager.AppSettings["AddBusinessProcessConsoleTracking"] == "true";
        static bool s_AddActivityEventsTracking = ConfigurationManager.AppSettings["BusinessProcess_ActivityEventsTracking"] == "true";
        static int s_maxConcurrentWorkflowsPerDefinition;

        static BPDefinitionInitiator()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["BusinessProcess_MaxConcurrentWorkflowsPerDefinition"], out s_maxConcurrentWorkflowsPerDefinition))
                s_maxConcurrentWorkflowsPerDefinition = 10;
        }

        BPDefinition _definition;

        static IBPInstanceDataManager s_instanceDataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
        static IBPEventDataManager s_eventDataManager = BPDataManagerFactory.GetDataManager<IBPEventDataManager>();
        Activity _workflowDefinition;

        //static SqlWorkflowInstanceStore s_InstanceStore = new SqlWorkflowInstanceStore(ConfigurationManager.ConnectionStrings["WFPersistence"].ConnectionString);

        private ConcurrentDictionary<long, BPRunningInstance> _runningInstances = new ConcurrentDictionary<long, BPRunningInstance>();



        BPDefinitionManager _definitionManager = new BPDefinitionManager();
        Guid _definitionId;
        int _maxNbOfThreads;

        static IEnumerable<BPInstanceStatus> s_acceptableBPStatusesToRun = new BPInstanceStatus[] { BPInstanceStatus.New, BPInstanceStatus.Postponed };
        public BPDefinitionInitiator(BusinessProcessRuntime runtime, BPDefinition definition)
        {
            definition.WorkflowType.ThrowIfNull("bpDefinition.WorkflowType", definition.BPDefinitionID);
            _definition = definition;
            _definitionId = definition.BPDefinitionID;
            try
            {
                _workflowDefinition = Activator.CreateInstance(definition.WorkflowType) as Activity;
            }
            catch (Exception ex)
            {
                Vanrise.Common.LoggerFactory.GetLogger().WriteError("Unable to create instance from type '{0}' for BPDefinitionID: '{1}'", definition.WorkflowType, definition.BPDefinitionID);
                throw;
            }
            if (_workflowDefinition == null)
                throw new Exception(String.Format("'{0}' is not of type Activity", definition.WorkflowType));
            _maxNbOfThreads = _definition.Configuration != null && _definition.Configuration.MaxConcurrentWorkflows.HasValue ? _definition.Configuration.MaxConcurrentWorkflows.Value : s_maxConcurrentWorkflowsPerDefinition;
        }

        #endregion

        #region Workflow Execution

        Guid _lastReceivedServiceInstanceId;
        bool _isRunning;

        internal void RunPendingProcesses(Guid serviceInstanceId)
        {
            lock (this)
            {
                if (_isRunning)
                    return;
                _isRunning = true;
            }
            _lastReceivedServiceInstanceId = serviceInstanceId;
            try
            {
                _definition = _definitionManager.GetBPDefinition(_definitionId);
                int nbOfRunningInstances = GetNbOfRunningInstances();
                if (nbOfRunningInstances >= _maxNbOfThreads)
                    return;
                int nbOfThreads = _maxNbOfThreads - nbOfRunningInstances;
                List<BPInstance> pendingInstances = s_instanceDataManager.GetPendingInstances(_definitionId, s_acceptableBPStatusesToRun, nbOfThreads, serviceInstanceId);
                if (pendingInstances != null)
                {
                    foreach (var instance in pendingInstances)
                    {
                        RunProcess(instance);
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

        bool _isTriggerPendingEventsRunning;
        internal void TriggerPendingEvents(Guid serviceInstanceId)
        {
            if (_isTriggerPendingEventsRunning)
                return;
            Task task = new Task(() =>
            {
                lock (this)
                {
                    if (_isTriggerPendingEventsRunning)
                        return;
                    _isTriggerPendingEventsRunning = true;
                }
                try
                {
                    if (this._definition.Configuration.IsPersistable)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        if (_runningInstances.Count > 0)
                        {
                            IEnumerable<BPEvent> events = s_eventDataManager.GetInstancesEvents(_runningInstances.Select(itm => itm.Key).ToList());
                            if (events != null)
                            {
                                foreach (var evnt in events)
                                {
                                    TriggerWFEvent(evnt.ProcessInstanceID, evnt.Bookmark, evnt.Payload);
                                    s_eventDataManager.DeleteEvent(evnt.BPEventID);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    lock (this)
                    {
                        _isTriggerPendingEventsRunning = false;
                    }
                }
            });

            task.Start();
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

            var sharedInstanceData = new BPSharedInstanceData(bpInstance, _definition);
            wfApp.Extensions.Add(sharedInstanceData);

            if (s_AddConsoleTracking)
                wfApp.Extensions.Add(ConsoleTracking.Instance);
            if (s_AddActivityEventsTracking)
                wfApp.Extensions.Add(new ActivityEventsTracking(bpInstance));

            wfApp.Completed = (e) =>
            {
                sharedInstanceData.ClearCacheManagers();
                OnWorkflowCompleted(bpInstance, e);
            };
            wfApp.Idle = (e) =>
                {
                    if (e.Bookmarks != null && e.Bookmarks.Count > 0)
                    {
                        runningInstance.IsIdle = true;
                    }
                };


            //wfApp.InstanceStore = s_InstanceStore;
            //wfApp.PersistableIdle = delegate(WorkflowApplicationIdleEventArgs e)
            //{
            //    return PersistableIdleAction.Persist;
            //};

            bpInstance.Status = BPInstanceStatus.Running;
            UpdateProcessStatus(bpInstance, wfApp.Id);
            wfApp.Run();
            string logEventType = bpInstance.InputArgument.GetDefinitionTitle();
            string processTitle = bpInstance.Title;
            LoggerFactory.GetLogger().WriteEntry(logEventType, GetGeneralLogViewRequiredPermissionSetId(bpInstance), LogEntryType.Information, "Process '{0}' started", processTitle);
        }

        void OnWorkflowCompleted(BPInstance bpInstance, WorkflowApplicationCompletedEventArgs e)
        {
            BPRunningInstance dummy;
            _runningInstances.TryRemove(bpInstance.ProcessInstanceID, out dummy);
            string logEventType = bpInstance.InputArgument.GetDefinitionTitle();
            string processTitle = bpInstance.Title;
            Exception terminationException = null;
            if (e.CompletionState == ActivityInstanceState.Closed)
            {
                bpInstance.Status = BPInstanceStatus.Completed;
                UpdateProcessStatus(bpInstance);
                LoggerFactory.GetLogger().WriteEntry(logEventType, GetGeneralLogViewRequiredPermissionSetId(bpInstance), LogEntryType.Information, "Process '{0}' completed", processTitle);
            }
            else
            {
                terminationException = e.TerminationException;
                BPTrackingChannel.Current.WriteException(bpInstance.ProcessInstanceID, bpInstance.ParentProcessID, e.TerminationException);
                bpInstance.LastMessage = String.Format("Workflow Finished Unsuccessfully. Status: {0}. Error: {1}", e.CompletionState, e.TerminationException);
                bpInstance.Status = BPInstanceStatus.Aborted;
                UpdateProcessStatus(bpInstance);

                Exception finalException = Utilities.WrapException(e.TerminationException, String.Format("Process '{0}' failed", processTitle));
                LoggerFactory.GetExceptionLogger().WriteException(logEventType, GetGeneralLogViewRequiredPermissionSetId(bpInstance), finalException);
                Console.WriteLine("{0}: {1}", DateTime.Now, bpInstance.LastMessage);

                BPDataManagerFactory.GetDataManager<IBPTaskDataManager>().CancelNotCompletedTasks(bpInstance.ProcessInstanceID);
            }

            if (BPInstanceStatusAttribute.GetAttribute(bpInstance.Status).IsClosed)
            {
                object processOutput = null;
                if (e.Outputs != null)
                    e.Outputs.TryGetValue("Output", out processOutput);

                if (bpInstance.ParentProcessID.HasValue)
                    NotifyParentBPChildCompleted(bpInstance.ProcessInstanceID, bpInstance.ParentProcessID.Value, bpInstance.Status, bpInstance.LastMessage, terminationException, processOutput);

                //if (bpInstance.CompletionNotifier != null)
                NotifyBPCompleted(_definition, bpInstance, processOutput);
            }

            //_instanceDataManager.UnlockProcessInstance(bpInstance.ProcessInstanceID, RunningProcessManager.CurrentProcess.ProcessId);
            RunPendingProcesses(_lastReceivedServiceInstanceId);
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
            BPDefinitionBPExecutionCompletedContext context = new BPDefinitionBPExecutionCompletedContext() { BPInstance = bpInstance };
            new BPDefinitionManager().GetBPDefinitionExtendedSettings(bpDefinition).OnBPExecutionCompleted(context);

            BPDataManagerFactory.GetDataManager<IBPTaskDataManager>().CancelNotCompletedTasks(bpInstance.ProcessInstanceID);
        }

        void TriggerWFEvent(long processInstanceId, string bookmarkName, object eventData)
        {
            BPRunningInstance runningInstance;
            if (_runningInstances.TryGetValue(processInstanceId, out runningInstance))
            {
                runningInstance.IsIdle = false;
                runningInstance.WFApplication.ResumeBookmark(bookmarkName, eventData);
                BPTrackingChannel.Current.WriteTrackingMessage(new BPTrackingMessage
                {
                    ProcessInstanceId = processInstanceId,
                    ParentProcessId = runningInstance.BPInstance.ParentProcessID,
                    TrackingMessage = String.Format("Event '{0}' triggerred", bookmarkName),
                    Severity = LogEntryType.Verbose,
                    EventTime = DateTime.Now
                });
            }
        }

        void UpdateProcessStatus(BPInstance bpInstance, Guid? workflowInstanceId = null)
        {
            UpdateProcessStatus(bpInstance.ProcessInstanceID, bpInstance.ParentProcessID, bpInstance.Status, bpInstance.LastMessage, workflowInstanceId);
        }

        internal static void UpdateProcessStatus(long processInstanceId, long? parentProcessId, BPInstanceStatus status, string errorMessage, Guid? workflowInstanceId = null)
        {
            s_instanceDataManager.UpdateInstanceStatus(processInstanceId, status, errorMessage, workflowInstanceId);
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


    }
}
