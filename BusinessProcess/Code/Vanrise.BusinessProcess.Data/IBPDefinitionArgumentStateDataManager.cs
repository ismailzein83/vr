using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPDefinitionArgumentStateDataManager : IDataManager
    {
        List<BPDefinitionArgumentState> GetBPDefinitionArgumentStates();

        bool InsertOrUpdateBPDefinitionArgumentState(BPDefinitionArgumentState bpDefinitionArgumentState);
    }
}