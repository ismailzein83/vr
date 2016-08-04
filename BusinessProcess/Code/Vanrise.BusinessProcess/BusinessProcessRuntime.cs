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
using Vanrise.Runtime;
using Vanrise.BusinessProcess.Business;

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
        }
        
        BPDefinitionManager _definitionManager = new BPDefinitionManager();
        IBPInstanceDataManager _instanceDataManager = BPDataManagerFactory.GetDataManager<IBPInstanceDataManager>();

        Dictionary<int, BPDefinitionInitiator> _processDefinitionInitiators;

        internal static IEnumerable<BPInstanceStatus> s_acceptableBPStatusesToRun = new BPInstanceStatus[] { BPInstanceStatus.New, BPInstanceStatus.ProcessFailed };

        #endregion

        public void TerminatePendingProcesses()
        {
            RunningProcessManager runningProcessManager = new RunningProcessManager();
            IEnumerable<int> runningRuntimeProcessesIds = runningProcessManager.GetCachedRunningProcesses().Select(itm => itm.ProcessId);
            var openStatuses = BPInstanceStatusAttribute.GetNonClosedStatuses();
            _instanceDataManager.SetRunningStatusTerminated(BPInstanceStatus.Running, runningRuntimeProcessesIds);
            //_instanceDataManager.SetChildrenStatusesTerminated(BPInstanceStatusAttribute.GetNonClosedStatuses(), runningRuntimeProcessesIds);
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
            task.Start();
        }

        public void ExecutePendings()
        {
            GC.Collect();
            LoadProcessDefinitionInitiators();
            foreach (var processInitiator in this._processDefinitionInitiators.Values)
            {
                processInitiator.RunPendingProcesses();
            }
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
            task.Start();
        }
        
        public void TriggerPendingEvents()
        {
            GC.Collect();
            foreach(var processInitiator in this._processDefinitionInitiators.Values)
            {
                processInitiator.TriggerPendingEvents();
            }
        }

        #endregion

        #region Private Methods

        private void LoadProcessDefinitionInitiators()
        {
            lock (this)
            {
                if (_processDefinitionInitiators == null)
                    _processDefinitionInitiators = new Dictionary<int, BPDefinitionInitiator>();
                var definitions = _definitionManager.GetBPDefinitions();
                foreach (var bpDefinition in definitions)
                {
                    if (!_processDefinitionInitiators.ContainsKey(bpDefinition.BPDefinitionID))
                        _processDefinitionInitiators.Add(bpDefinition.BPDefinitionID, new BPDefinitionInitiator(this, bpDefinition));
                }
            }
        }

        #endregion
    }
}