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
                s_maxConcurrentWorkflowsPerDefinition = 20;
        }

        BPDefinition _definition;

        IBPInstanceDataManager _instanceDataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();
        IBPEventDataManager _eventDataManager = BPDataManagerFactory.GetDataManager<IBPEventDataManager>();
        Activity _workflowDefinition;

        RunningProcessManager _runningProcessManager = new RunningProcessManager();
        //static SqlWorkflowInstanceStore s_InstanceStore = new SqlWorkflowInstanceStore(ConfigurationManager.ConnectionStrings["WFPersistence"].ConnectionString);

        private ConcurrentDictionary<long, BPRunningInstance> _runningInstances = new ConcurrentDictionary<long, BPRunningInstance>();

        

        BPDefinitionManager _definitionManager = new BPDefinitionManager();
        int _definitionId;

        BPInstanceManager _instanceManager = new BPInstanceManager();

        public BPDefinitionInitiator(BusinessProcessRuntime runtime, BPDefinition definition)
        {
            _definition = definition;
            _definitionId = definition.BPDefinitionID;
            _workflowDefinition = Activator.CreateInstance(definition.WorkflowType) as Activity;
            if (_workflowDefinition == null)
                throw new Exception(String.Format("'{0}' is not of type Activity", definition.WorkflowType));
        }

        #endregion

        #region Internal Methods
        
        
        
        #endregion

        #region Workflow Execution

        internal void RunPendingProcesses()
        {
            _definition = _definitionManager.GetBPDefinition(_definitionId);
            int nbOfThreads = _definition.Configuration != null && _definition.Configuration.MaxConcurrentWorkflows.HasValue ? _definition.Configuration.MaxConcurrentWorkflows.Value : s_maxConcurrentWorkflowsPerDefinition;
            if (_runningInstances.Where(itm => itm.Value.BPInstance.Status == BPInstanceStatus.Running && !itm.Value.IsIdle).Count() >= nbOfThreads)
                return;
            int currentRuntimeProcessId = RunningProcessManager.CurrentProcess.ProcessId;
            IEnumerable<int> runningRuntimeProcessesIds = _runningProcessManager.GetCachedRunningProcesses().Select(itm => itm.ProcessId);
            List<BPInstance> pendingInstances = _instanceDataManager.GetPendingInstances(_definitionId, BusinessProcessRuntime.s_acceptableBPStatusesToRun, nbOfThreads, currentRuntimeProcessId, runningRuntimeProcessesIds);
            if(pendingInstances != null)
            {
                foreach(var instance in pendingInstances)
                {                    
                    TryRunProcess(instance, currentRuntimeProcessId, runningRuntimeProcessesIds);
                }
            }
        }

        private bool IsParentProcessRunning(long parentProcessInstanceId)
        {
            BPInstanceStatus parentStatus;
            if (_instanceManager.TryGetBPInstanceStatus(parentProcessInstanceId, out parentStatus))
            {
                return parentStatus == BPInstanceStatus.Running;
            }
            else
                return false;
        }


        internal void TriggerPendingEvents()
        {            
            if (this._definition.Configuration.IsPersistable)
            {
                IEnumerable<BPEvent> events = _eventDataManager.GetDefinitionEvents(this._definitionId);
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
                    IEnumerable<BPEvent> events = _eventDataManager.GetInstancesEvents(this._definitionId, idledInstancesIds);
                    if(events != null)
                    {
                        foreach(var evnt in events)
                        {
                            TriggerWFEvent(evnt.ProcessInstanceID, evnt.Bookmark, evnt.Payload);
                            _eventDataManager.DeleteEvent(evnt.BPEventID);
                        }
                    }
                }
                
            }
        }

        #endregion

        #region Private Methods

        private void TryRunProcess(BPInstance bpInstance, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds)
        {
            IDictionary<string, object> inputs = null;
            if (bpInstance.InputArgument != null)
            {
                inputs = new Dictionary<string, object>();
                inputs.Add("Input", bpInstance.InputArgument);
            }

            WorkflowApplication wfApp = inputs != null ? new WorkflowApplication(_workflowDefinition, inputs) : new WorkflowApplication(_workflowDefinition);
            if (!_instanceDataManager.TryLockProcessInstance(bpInstance.ProcessInstanceID, wfApp.Id, currentRuntimeProcessId, runningRuntimeProcessesIds, BusinessProcessRuntime.s_acceptableBPStatusesToRun))
                return;

            if (bpInstance.ParentProcessID.HasValue && !IsParentProcessRunning(bpInstance.ParentProcessID.Value))
            {
                bpInstance.Status = BPInstanceStatus.Terminated;
                UpdateProcessStatus(bpInstance);
                return;
            }

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
            UpdateProcessStatus(bpInstance);
            wfApp.Run();
        }
                
        void OnWorkflowCompleted(BPInstance bpInstance, WorkflowApplicationCompletedEventArgs e)
        {
            if (e.CompletionState == ActivityInstanceState.Closed)
            {
                bpInstance.Status = BPInstanceStatus.Completed;
                UpdateProcessStatus(bpInstance);
            }
            else
            {
                bpInstance.LastMessage = String.Format("Workflow Finished Unsuccessfully. Status: {0}. Error: {1}", e.CompletionState, e.TerminationException);
                bpInstance.RetryCount++;
                bpInstance.Status = _definition.Configuration != null && _definition.Configuration.RetryOnProcessFailed && bpInstance.RetryCount < 3
                    ? BPInstanceStatus.ProcessFailed : BPInstanceStatus.Aborted;
                UpdateProcessStatus(bpInstance);
                Console.WriteLine("{0}: {1}", DateTime.Now, bpInstance.LastMessage);
            }

            if (bpInstance.ParentProcessID.HasValue && BPInstanceStatusAttribute.GetAttribute(bpInstance.Status).IsClosed)
            {
                object processOutput = null;
                if (e.Outputs != null)
                    e.Outputs.TryGetValue("Output", out processOutput);

                var eventData = new ProcessCompletedEventPayload
                {
                    ProcessStatus = bpInstance.Status,
                    LastProcessMessage = bpInstance.LastMessage,
                    ProcessOutput = processOutput
                };
                _eventDataManager.InsertEvent(bpInstance.ParentProcessID.Value, bpInstance.ProcessInstanceID.ToString(), eventData);
            }

            _instanceDataManager.UnlockProcessInstance(bpInstance.ProcessInstanceID, RunningProcessManager.CurrentProcess.ProcessId);
            BPRunningInstance dummy;
            _runningInstances.TryRemove(bpInstance.ProcessInstanceID, out dummy);
            GC.Collect();
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

        void UpdateProcessStatus(BPInstance bpInstance)
        {
            _instanceDataManager.UpdateInstanceStatus(bpInstance.ProcessInstanceID, bpInstance.Status, bpInstance.LastMessage, bpInstance.RetryCount);
            LogEntryType statusChangedTrackingSeverity = BPInstanceStatusAttribute.GetAttribute(bpInstance.Status).TrackingSeverity;
            BPTrackingChannel.Current.WriteTrackingMessage(new BPTrackingMessage
            {
                ProcessInstanceId = bpInstance.ProcessInstanceID,
                ParentProcessId = bpInstance.ParentProcessID,
                TrackingMessage = String.Format("Status changed to '{0}'. {1}", bpInstance.Status, statusChangedTrackingSeverity == LogEntryType.Error || statusChangedTrackingSeverity == LogEntryType.Warning ? bpInstance.LastMessage : null),
                Severity = statusChangedTrackingSeverity,
                EventTime = DateTime.Now
            });
        }

        #endregion


    }
}
