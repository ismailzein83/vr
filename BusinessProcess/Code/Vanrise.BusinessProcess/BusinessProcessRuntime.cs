using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;
using System.Activities;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using System.Threading.Tasks;

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

        Dictionary<string, BPDefinitionInitiator> _processDefinitionInitiators;

        #endregion

        #region Internal Methods
        internal CreateProcessOutput CreateNewProcess(CreateProcessInput input)
        {

            InitializeIfNotInitialized();

            BPDefinitionInitiator processInitiator;
            if (!_processDefinitionInitiators.TryGetValue(input.ProcessName, out processInitiator))
                throw new ArgumentException(String.Format("'{0}' not found in Process Definitions list", input.ProcessName));

            string processTitle = processInitiator.WorkflowDefinition.GetTitle(input);
            long processInstanceId = _dataManager.InsertInstance(processTitle, input.ParentProcessID, processInitiator.Definition.BPDefinitionID, input.InputArguments, BPInstanceStatus.New);
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
                    Result = CreateProcessResult.Succeeded
                };
            return output;
        }

        internal TriggerProcessEventOutput TriggerProcessEvent(TriggerProcessEventInput input)
        {
            TriggerProcessEventOutput output = new TriggerProcessEventOutput();
            if (_dataManager.InsertEvent(input.ProcessInstanceId, input.BookmarkName, input.EventData) > 0)
                output.Result = TriggerProcessEventResult.Succeeded;
            else
                output.Result = TriggerProcessEventResult.ProcessInstanceNotExists;
            return output;
        }

        #endregion

        public void TerminatePendingProcesses()
        {
            InitializeIfNotInitialized();
            _dataManager.LoadPendingProcesses((bpInstance) =>
                {
                    _dataManager.UpdateInstanceStatus(bpInstance.ProcessInstanceID, BPInstanceStatus.Terminated, null, bpInstance.RetryCount);
                });
        }

        #region Process Execution

        bool _isExecutePendingsRunning;

        internal void ExecutePendingsIfIdleAsync()
        {
            lock (this)
            {
                if (_isExecutePendingsRunning)
                    return;
                _isExecutePendingsRunning = true;
            }
            Task task = new Task(() =>
            {
                try
                {
                    ExecutePendings();
                }
                finally
                {
                    lock (this)
                    {
                        _isExecutePendingsRunning = false;
                    }
                }
            });
        }

        public void ExecutePendings()
        {
            GC.Collect();
            InitializeIfNotInitialized();
            LoadAndRunPendingProcesses();
        }

        bool _isTriggerPendingEventsRunning;

        internal void TriggerPendingEventsIfIdleAsync()
        {
            lock (this)
            {
                if (_isTriggerPendingEventsRunning)
                    return;
                _isTriggerPendingEventsRunning = true;
            }
            Task task = new Task(() =>
            {
                try
                {
                    TriggerPendingEvents();
                }
                finally
                {
                    lock (this)
                    {
                        _isTriggerPendingEventsRunning = false;
                    }
                }
            });
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

                _processDefinitionInitiators = new Dictionary<string, BPDefinitionInitiator>();
                var definitions = _dataManager.GetDefinitions();
                foreach (var bpDefinition in definitions)
                {
                    _processDefinitionInitiators.Add(bpDefinition.Name, new BPDefinitionInitiator(bpDefinition));
                }

                _isInitialized = true;
                BPService.Start();
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