using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPDefinitionDataManager : IDataManager
    {
        List<BPDefinition> GetBPDefinitions();

        bool AreBPDefinitionsUpdated(ref object updateHandle);

        bool UpdateBPDefinition(BPDefinition bPDefinition);
    }
}
