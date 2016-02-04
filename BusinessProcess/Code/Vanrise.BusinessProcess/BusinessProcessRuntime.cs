﻿using System;
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

        internal static IEnumerable<BPInstanceStatus> s_acceptableBPStatusesToRun = new BPInstanceStatus[] { BPInstanceStatus.New, BPInstanceStatus.ProcessFailed };

        #endregion

        public void TerminatePendingProcesses()
        {
            InitializeIfNotInitialized();
            RunningProcessManager runningProcessManager = new RunningProcessManager();
            IEnumerable<int> runningRuntimeProcessesIds = runningProcessManager.GetCachedRunningProcesses(new TimeSpan(0, 0, 15)).Select(itm => itm.ProcessId);
            _dataManager.UpdateProcessInstancesStatus(BPInstanceStatus.Running, BPInstanceStatus.Terminated, runningRuntimeProcessesIds);
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
            task.Start();
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

                _processDefinitionInitiators = new Dictionary<string, BPDefinitionInitiator>();
                var definitions = _dataManager.GetDefinitions();
                foreach (var bpDefinition in definitions)
                {
                    _processDefinitionInitiators.Add(bpDefinition.Name, new BPDefinitionInitiator(this, bpDefinition));
                }

                _isInitialized = true;
            }
        }

        List<long> _loadedInstanceIds = new List<long>();

        private void LoadAndRunPendingProcesses()
        {
            List<long> loadedInstanceIds = new List<long>();
            lock (_loadedInstanceIds)
                loadedInstanceIds.AddRange(_loadedInstanceIds);
            _dataManager.LoadPendingProcesses(loadedInstanceIds, s_acceptableBPStatusesToRun, (bp) =>
            {
                BPDefinitionInitiator processInitiator = _processDefinitionInitiators.Values.First(itm => itm.Definition.BPDefinitionID == bp.DefinitionID);
                lock (_loadedInstanceIds)
                    _loadedInstanceIds.Add(bp.ProcessInstanceID);
                processInitiator.ScheduleNewProcessRun(bp);
                processInitiator.RunProcessExecutionIfNeeded();
            });
        }

        internal void SetProcessInstanceNotLoaded(BPInstance instance)
        {
            lock (_loadedInstanceIds)
            {
                if (_loadedInstanceIds.Contains(instance.ProcessInstanceID))
                    _loadedInstanceIds.Remove(instance.ProcessInstanceID);
            }
        }

        long _lastRetrievedEventId;
        private void LoadAndSendPendingEvents()
        {
            _dataManager.LoadPendingEvents(_lastRetrievedEventId, (evnt) =>
            {
                BPDefinitionInitiator processInitiator = _processDefinitionInitiators.Values.First(itm => itm.Definition.BPDefinitionID == evnt.ProcessDefinitionID);
                if (processInitiator.TriggerWFEvent(evnt.ProcessInstanceID, evnt.Bookmark, evnt.Payload))
                    _dataManager.DeleteEvent(evnt.BPEventID);
                if (evnt.BPEventID > _lastRetrievedEventId)
                    _lastRetrievedEventId = evnt.BPEventID;
            });
        }

        #endregion
    }
}