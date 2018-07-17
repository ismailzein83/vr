using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Business
{
    public class BPDefintionArgumentStateManager
    {
        public BPDefinitionArgumentState GetBPDefinitionArgumentState(Guid bpDefinitionId)
        {
            var bpDefinitionArgumentStates = GetAllBPDefinitionArgumentStates();
            if (bpDefinitionArgumentStates == null)
                return null;

            return bpDefinitionArgumentStates.GetRecord(bpDefinitionId);
        }

        public Dictionary<Guid, BPDefinitionArgumentState> GetAllBPDefinitionArgumentStates()
        {
            IBPDefinitionArgumentStateDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDefinitionArgumentStateDataManager>();
            IEnumerable<BPDefinitionArgumentState> bpDefinitionArgumentStates = dataManager.GetBPDefinitionArgumentStates();
            if (bpDefinitionArgumentStates == null)
                return null;

            return bpDefinitionArgumentStates.ToDictionary(cn => cn.BPDefinitionID, cn => cn);
        }

        public bool InsertOrUpdateBPDefinitionArgumentState(BPDefinitionArgumentState bpDefinitionArgumentState)
        {
             IBPDefinitionArgumentStateDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDefinitionArgumentStateDataManager>();
             return dataManager.InsertOrUpdateBPDefinitionArgumentState(bpDefinitionArgumentState);
        }
    }
}