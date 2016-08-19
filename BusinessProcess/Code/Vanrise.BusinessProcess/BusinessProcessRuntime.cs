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


        #endregion
        
        #region Process Execution
        
        public void ExecutePendings(int bpDefinitionId, Guid serviceInstanceId)
        {
            //GC.Collect();
            LoadProcessDefinitionInitiators();
            BPDefinitionInitiator bpDefinitionInitiator;
            if (!this._processDefinitionInitiators.TryGetValue(bpDefinitionId, out bpDefinitionInitiator))
                throw new NullReferenceException(String.Format("bpDefinitionInitiator. bpDefinitionId '{0}'", bpDefinitionId));
            bpDefinitionInitiator.RunPendingProcesses(serviceInstanceId);
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
            LoadProcessDefinitionInitiators();
            //GC.Collect();
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