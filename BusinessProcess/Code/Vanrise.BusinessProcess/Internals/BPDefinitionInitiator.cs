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
                s_maxConcurrentWorkflowsPerDefinition = 5;
        }

        BPDefinition _definition;

        static IBPInstanceDataManager s_instanceDataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
        static IBPEventDataManager s_eventDataManager = BPDataManagerFactory.GetDataManager<IBPEventDataManager>();
        Activity _workflowDefinition;

        //static SqlWorkflowInstanceStore s_InstanceStore = new SqlWorkflowInstanceStore(ConfigurationManager.ConnectionStrings["WFPersistence"].ConnectionString);

        private ConcurrentDictionary<long, BPRunningInstance> _runningInstances = new ConcurrentDictionary<long, BPRunningInstance>();

        

        BPDefinitionManager _definitionManager = new BPDefinitionManager();
        int _definitionId;

        static IEnumerable<BPInstanceStatus> s_acceptableBPStatusesToRun = new BPInstanceStatus[] { BPInstanceStatus.New };
        public BPDefinitionInitiator(BusinessProcessRuntime runtime, BPDefinition definition)
        {
            _definition = definition;
            _definitionId = definition.BPDefinitionID;
            _workflowDefinition = Activator.CreateInstance(definition.WorkflowType) as Activity;
            if (_workflowDefinition == null)
                throw new Exception(String.Format("'{0}' is not of type Activity", definition.WorkflowType));
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
                int nbOfThreads = _definition.Configuration != null && _definition.Configuration.MaxConcurrentWorkflows.HasValue ? _definition.Configuration.MaxConcurrentWorkflows.Value : s_maxConcurrentWorkflowsPerDefinition;
                if (_runningInstances.Where(itm => itm.Value.BPInstance.Status == BPInstanceStatus.Running && !itm.Value.IsIdle).Count() >= nbOfThreads)
                    return;
                List<BPInstance> pendingInstances = s_instanceDataManager.GetPendingInstances(_definitionId, s_acceptableBPStatusesToRun, nbOfThreads, serviceInstanceId);
                if (pendingInstances != null)
                {
                    foreach (var instance in pendingInstances)
                    {
                        TryRunProcess(instance);
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

        internal void TriggerPendingEvents()
        {            
            if (this._definition.Configuration.IsPersistable)
            {
                IEnumerable<BPEvent> events = s_eventDataManager.GetDefinitionEvents(this._definitionId);
                if(events != null)
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                List<long> idledInstancesIds = _runningInstances.Where(itm => itm.Value.IsIdle).Select(itm => itm.Key).ToList();
                if(idledInstancesIds.Count > 0)
                {
                    IEnumerable<BPEvent> events = s_eventDataManager.GetInstancesEvents(this._definitionId, idledInstancesIds);
                    if(events != null)
                    {
                        foreach(var evnt in events)
                        {
                            TriggerWFEvent(evnt.ProcessInstanceID, evnt.Bookmark, evnt.Payload);
                            s_eventDataManager.DeleteEvent(evnt.BPEventID);
                        }
                    }
                }
                
            }
        }

        #endregion

        #region Private Methods

        private void TryRunProcess(BPInstance bpInstance)
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

            var sharedInstanceData = new BPSharedInstanceData()
            {
                InstanceInfo = bpInstance
            };
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
        }
                
        void OnWorkflowCompleted(BPInstance bpInstance, WorkflowApplicationCompletedEventArgs e)
        {
            BPRunningInstance dummy;
            _runningInstances.TryRemove(bpInstance.ProcessInstanceID, out dummy);
            if (e.CompletionState == ActivityInstanceState.Closed)
            {
                bpInstance.Status = BPInstanceStatus.Completed;
                UpdateProcessStatus(bpInstance);
            }
            else
            {
                bpInstance.LastMessage = String.Format("Workflow Finished Unsuccessfully. Status: {0}. Error: {1}", e.CompletionState, e.TerminationException);                
                bpInstance.Status = BPInstanceStatus.Aborted;
                UpdateProcessStatus(bpInstance);
                Console.WriteLine("{0}: {1}", DateTime.Now, bpInstance.LastMessage);
            }

            if (bpInstance.ParentProcessID.HasValue && BPInstanceStatusAttribute.GetAttribute(bpInstance.Status).IsClosed)
            {
                object processOutput = null;
                if (e.Outputs != null)
                    e.Outputs.TryGetValue("Output", out processOutput);

                NotifyParentBPChildCompleted(bpInstance.ProcessInstanceID, bpInstance.ParentProcessID.Value, bpInstance.Status, bpInstance.LastMessage, processOutput);
            }

            //_instanceDataManager.UnlockProcessInstance(bpInstance.ProcessInstanceID, RunningProcessManager.CurrentProcess.ProcessId);
            RunPendingProcesses(_lastReceivedServiceInstanceId);
            GC.Collect();
        }

        internal static void NotifyParentBPChildCompleted(long bpInstanceId, long parentBPInstanecId, BPInstanceStatus status, string errorMessage, object processOutput)
        {
            var eventData = new ProcessCompletedEventPayload
            {
                ProcessStatus = status,
                LastProcessMessage = errorMessage,
                ProcessOutput = processOutput
            };
            s_eventDataManager.InsertEvent(parentBPInstanecId, bpInstanceId.ToString(), eventData);
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
                TrackingMessage = String.Format("Status changed to '{0}'. {1}", status, statusChangedTrackingSeverity == LogEntryType.Error || statusChangedTrackingSeverity == LogEntryType.Warning ? errorMessage : null),
                Severity = statusChangedTrackingSeverity,
                EventTime = DateTime.Now
            });
        }

        #endregion


    }
}
