using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPBusinessRuleDefinitionDataManager : IDataManager
    {
        List<BPBusinessRuleDefinition> GetBPBusinessRuleDefinitions();

        bool AreBPBusinessRuleDefinitionsUpdated(ref object lastReceivedDataInfo);
    }
}