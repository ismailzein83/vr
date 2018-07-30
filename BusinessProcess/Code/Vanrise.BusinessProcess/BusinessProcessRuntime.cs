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
    internal class BusinessProcessRuntime
    {


        #region ctor/Local Variables

        Guid _serviceInstanceId;

        internal BusinessProcessRuntime(Guid serviceInstanceId)
        {
            _serviceInstanceId = serviceInstanceId;
        }

        BPDefinitionManager _definitionManager = new BPDefinitionManager();

        Dictionary<Guid, BPDefinitionInitiator> _processDefinitionInitiators;


        #endregion

        #region Process Execution

        public void ExecutePendings(Guid bpDefinitionId)
        {
            LoadProcessDefinitionInitiators();
            BPDefinitionInitiator bpDefinitionInitiator;
            if (!this._processDefinitionInitiators.TryGetValue(bpDefinitionId, out bpDefinitionInitiator))
                throw new NullReferenceException(String.Format("bpDefinitionInitiator. bpDefinitionId '{0}'", bpDefinitionId));
            bpDefinitionInitiator.RunPendingProcesses();
        }

        public void ExecuteCancellationRequest(BPInstanceCancellationRequest cancellationRequest)
        {
            LoadProcessDefinitionInitiators();
            BPDefinitionInitiator bpDefinitionInitiator;
            if (!this._processDefinitionInitiators.TryGetValue(cancellationRequest.BPDefinitionId, out bpDefinitionInitiator))
                throw new NullReferenceException(String.Format("bpDefinitionInitiator. bpDefinitionId '{0}'", cancellationRequest.BPDefinitionId));
            bpDefinitionInitiator.ExecuteCancellationRequest(cancellationRequest.BPInstanceId, cancellationRequest.Reason);
        }

        #endregion

        #region Private Methods

        private void LoadProcessDefinitionInitiators()
        {
            lock (this)
            {
                if (_processDefinitionInitiators == null)
                    _processDefinitionInitiators = new Dictionary<Guid, BPDefinitionInitiator>();
                var definitions = _definitionManager.GetBPDefinitions();
                foreach (var bpDefinition in definitions)
                {
                    if (!_processDefinitionInitiators.ContainsKey(bpDefinition.BPDefinitionID))
                        _processDefinitionInitiators.Add(bpDefinition.BPDefinitionID, new BPDefinitionInitiator(_serviceInstanceId, bpDefinition));
                }
            }
        }

        #endregion
    }
}