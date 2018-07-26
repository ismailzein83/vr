using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPDefinitionDataManager : IDataManager
    {
        List<BPDefinition> GetBPDefinitions();

        bool InsertBPDefinition(BPDefinition bpDefinition);

        bool UpdateBPDefinition(BPDefinition bPDefinition);

        bool AreBPDefinitionsUpdated(ref object updateHandle);
    }
}