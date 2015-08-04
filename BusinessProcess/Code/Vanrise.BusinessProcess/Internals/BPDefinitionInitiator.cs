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
using Vanrise.Runtime;

namespace Vanrise.BusinessProcess
{
    internal class BPDefinitionInitiator
    {
        #region Members/Ctor

        BPDefinition _definition;
        public BPDefinition Definition
        {
            get
            {
                return _definition;
            }
        }
        
        IBPDataManager _dataManager;
        IBPTrackingDataManager _trackingDataManager;
        Activity _workflowDefinition;
        Task _processExecutionThread;
        //Semaphore _semaphore;
        int? _nbOfThreads;
        BusinessProcessRuntime _runtime;
        RunningProcessManager _runningProcessManager;
        //static SqlWorkflowInstanceStore s_InstanceStore = new SqlWorkflowInstanceStore(ConfigurationManager.ConnectionStrings["WFPersistence"].ConnectionString);

        private ConcurrentDictionary<long, BPRunningInstance> _runningInstances = new ConcurrentDictionary<long, BPRunningInstance>();
        ConcurrentQueue<BPInstance> _qPendingInstances = new ConcurrentQueue<BPInstance>();
        static bool s_AddConsoleTracking = ConfigurationManager.AppSettings["AddBusinessProcessConsoleTracking"] == "true";

        public BPDefinitionInitiator(BusinessProcessRuntime runtime, BPDefinition definition)
        {
            _runtime = runtime;
            _runningProcessManager = new RunningProcessManager();
            _dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            _trackingDataManager = BPDataManagerFactory.GetDataManager<IBPTrackingDataManager>();
            _definition = definition;
            _workflowDefinition = Activator.CreateInstance(definition.WorkflowType) as Activity;            
            _nbOfThreads = _definition.Configuration != null && _definition.Configuration.MaxConcurrentWorkflows.HasValue ? _definition.Configuration.MaxConcurrentWorkflows.Value : default(int?);
            //if (_nbOfThreads.HasValue)
            //    _semaphore = new Semaphore(_nbOfThreads.Value, _nbOfThreads.Value);
        }

        #endregion

        #region Internal Methods

        internal void ScheduleNewProcessRun(BPInstance bpInstance)
        {
            _qPendingInstances.Enqueue(bpInstance);
        }

        internal void RunProcessExecutionIfNeeded()
        {
            lock (this)
            {
                if (_qPendingInstances.Count > 0 &&
                    (_processExecutionThread == null || _processExecutionThread.IsCompleted))
                {                    
                    _processExecutionThread = new Task(RunPendingProcesses);
                    _processExecutionThread.Start();
                }
            }
        }

        internal bool TriggerWFEvent(long processInstanceId, string bookmarkName, object eventData)
        {
            BPRunningInstance runningInstance ;
            if (_runningInstances.TryGetValue(processInstanceId, out runningInstance))
            {
                runningInstance.WFApplication.ResumeBookmark(bookmarkName, eventData);
                BPTrackingChannel.Current.WriteTrackingMessage(new BPTrackingMessage
                {
                    ProcessInstanceId = processInstanceId,
                    ParentProcessId = runningInstance.BPInstance.ParentProcessID,
                    TrackingMessage = String.Format("Event '{0}' triggerred", bookmarkName),
                    Severity = BPTrackingSeverity.Verbose,
                    EventTime = DateTime.Now
                });
                return true;
            }
            else
                return false;
        }

        #endregion

        #region Workflow Execution


        void RunPendingProcesses()
        {            
            while (_qPendingInstances.Count > 0)
            {
                BPInstance bpInstance = null;
                if(_qPendingInstances.TryDequeue(out bpInstance))
                {
                    if (_nbOfThreads.HasValue)
                    {
                        while (_runningInstances.Where(itm => itm.Value.BPInstance.Status == BPInstanceStatus.Running).Count() >= _nbOfThreads.Value)
                            Thread.Sleep(1000);
                    }
                    //if (_semaphore != null)
                    //    _semaphore.WaitOne();
                    //Task task = new Task(() => RunProcess(bpInstance)); 
                    //task.Start();

                    RunProcess(bpInstance);
                }
            }
        }

        
        void RunProcess(BPInstance bpInstance)
        {
            IDictionary<string, object> inputs = null;
            if (bpInstance.InputArgument != null)
            {
                inputs = new Dictionary<string, object>();
                inputs.Add("Input", bpInstance.InputArgument);
            }

            WorkflowApplication wfApp = inputs != null ? new WorkflowApplication(_workflowDefinition, inputs) : new WorkflowApplication(_workflowDefinition);

            int currentRuntimeProcessId = RunningProcessManager.CurrentProcess.ProcessId;
            IEnumerable<int> runningRuntimeProcessesIds = _runningProcessManager.GetCachedRunningProcesses(new TimeSpan(0, 0, 15)).Select(itm => itm.ProcessId);
            if (!_dataManager.TryLockProcessInstance(bpInstance.ProcessInstanceID, wfApp.Id, currentRuntimeProcessId, runningRuntimeProcessesIds, BusinessProcessRuntime.s_acceptableBPStatusesToRun))
                return;

            bpInstance.WorkflowInstanceID = wfApp.Id;
            BPRunningInstance runningInstance = new BPRunningInstance
            {
                BPInstance = bpInstance,
                WFApplication = wfApp
            };
            _runningInstances.TryAdd(bpInstance.ProcessInstanceID, runningInstance);

            var sharedInstanceData = new BPSharedInstanceData()
            {
                InstanceInfo = bpInstance
            };
            wfApp.Extensions.Add(sharedInstanceData);

            if (s_AddConsoleTracking)
                wfApp.Extensions.Add(ConsoleTracking.Instance);

            wfApp.Completed = (e) =>
                {
                    sharedInstanceData.ClearCacheManagers();
                    OnWorkflowCompleted(bpInstance, e);
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
            //if (_semaphore != null)
            //    _semaphore.Release();

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
                _dataManager.InsertEvent(bpInstance.ParentProcessID.Value, bpInstance.ProcessInstanceID.ToString(), eventData);
            }

            _runtime.SetProcessInstanceNotLoaded(bpInstance);
            _dataManager.UnlockProcessInstance(bpInstance.ProcessInstanceID, RunningProcessManager.CurrentProcess.ProcessId);
            BPRunningInstance dummy;
            _runningInstances.TryRemove(bpInstance.ProcessInstanceID, out dummy);
            GC.Collect();
        }

        #endregion

        #region Private Methods

        void UpdateProcessStatus(BPInstance bpInstance)
        {
            _dataManager.UpdateInstanceStatus(bpInstance.ProcessInstanceID, bpInstance.Status, bpInstance.LastMessage, bpInstance.RetryCount);
            BPTrackingSeverity statusChangedTrackingSeverity = BPInstanceStatusAttribute.GetAttribute(bpInstance.Status).TrackingSeverity;
            BPTrackingChannel.Current.WriteTrackingMessage(new BPTrackingMessage
            {
                ProcessInstanceId = bpInstance.ProcessInstanceID,
                ParentProcessId = bpInstance.ParentProcessID,
                TrackingMessage = String.Format("Status changed to '{0}'. {1}", bpInstance.Status, statusChangedTrackingSeverity == BPTrackingSeverity.Error || statusChangedTrackingSeverity == BPTrackingSeverity.Warning ? bpInstance.LastMessage : null),
                Severity = statusChangedTrackingSeverity,
                EventTime = DateTime.Now
            });
        }

        #endregion
    }
}
