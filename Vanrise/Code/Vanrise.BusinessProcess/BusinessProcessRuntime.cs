using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;
using System.Activities;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public class BusinessProcessRuntime
    {
        #region Singleton

        static BusinessProcessRuntime _current;
        public static BusinessProcessRuntime Current
        {
            get
            {
                return _current;
            }
        }

        static BusinessProcessRuntime()
        {
            _current = new BusinessProcessRuntime();
        }

        #endregion

        #region ctor/Local Variables

        private BusinessProcessRuntime()
        {
            _dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
        }

        IBPDataManager _dataManager;
        
        Dictionary<Type, BPDefinitionInitiator> _processDefinitionInitiators;

        #endregion

        public CreateProcessOutput CreateNewProcess<T>(CreateProcessInput input) where T : Activity, IBPWorkflow
        {

            InitializeIfNotInitialized();

            BPDefinitionInitiator processInitiator;
            if (!_processDefinitionInitiators.TryGetValue(typeof(T), out processInitiator))
                throw new ArgumentException(String.Format("'{0}' not found in Process Definitions list", typeof(T)));
            Guid processInstanceId = Guid.NewGuid();
            string processTitle = processInitiator.WorkflowDefinition.GetTitle(input);
            _dataManager.InsertInstance(processInstanceId, processTitle, input.ParentProcessID, processInitiator.Definition.BPDefinitionID, input.InputArguments, BPInstanceStatus.New);
            BPTrackingChannel.Current.WriteTrackingMessage(new BPTrackingMessage
            {
                ProcessInstanceId = processInstanceId,
                ParentProcessId = input.ParentProcessID,
                Message = String.Format("Process Created: {0}", processTitle),
                Severity = BPTrackingSeverity.Information,
                EventTime = DateTime.Now
            });
            CreateProcessOutput output = new CreateProcessOutput
                {
                    ProcessInstanceId = processInstanceId,
                    IsCreated = true
                };
            return output;
        }

        public void TriggerProcessEvent(Guid processInstanceId, string bookmarkName, object eventData)
        {            
            _dataManager.InsertEvent(processInstanceId, bookmarkName, eventData);
        }

        public void TerminatePendingProcesses()
        {
            InitializeIfNotInitialized();
            _dataManager.LoadPendingProcesses((bpInstance) =>
                {
                    _dataManager.UpdateInstanceStatus(bpInstance.ProcessInstanceID, BPInstanceStatus.Terminated, null, bpInstance.RetryCount);
                });
        }

        #region Process Execution

        public void ExecutePendings()
        {
            GC.Collect();
            InitializeIfNotInitialized();
            LoadAndRunPendingProcesses();
        }

        public void TriggerPendingEvents()
        {
            GC.Collect();
            InitializeIfNotInitialized();
            LoadAndSendPendingEvents();
        }

        #endregion

        #region Private Methods

        bool _isInitialized;
        private void InitializeIfNotInitialized()
        {
            lock (this)
            {
                if (_isInitialized)
                    return;

                _dataManager.ClearLoadedFlag();

                _processDefinitionInitiators = new Dictionary<Type, BPDefinitionInitiator>();
                var definitions = _dataManager.GetDefinitions();
                foreach (var bpDefinition in definitions)
                {
                    _processDefinitionInitiators.Add(bpDefinition.WorkflowType, new BPDefinitionInitiator(bpDefinition));
                }

                _isInitialized = true;
            }
        }

        private void LoadAndRunPendingProcesses()
        {
            _dataManager.LoadPendingProcesses((bp) =>
            {
                BPDefinitionInitiator processInitiator = _processDefinitionInitiators.Values.First(itm => itm.Definition.BPDefinitionID == bp.DefinitionID);
                _dataManager.UpdateLoadedFlag(bp.ProcessInstanceID, true);
                processInitiator.ScheduleNewProcessRun(bp);                
                processInitiator.RunProcessExecutionIfNeeded();
            });
        }

        private void LoadAndSendPendingEvents()
        {
            _dataManager.LoadPendingEvents((evnt) =>
            {
                BPDefinitionInitiator processInitiator = _processDefinitionInitiators.Values.First(itm => itm.Definition.BPDefinitionID == evnt.ProcessDefinitionID);
                processInitiator.TriggerWFEvent(evnt.ProcessInstanceID, evnt.Bookmark, evnt.Payload);
                _dataManager.DeleteEvent(evnt.BPEventID);
            });
        }
 
        #endregion
    }
}
