using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface INormalizationRuleDataManager : IDataManager
    {
        List<NormalizationRule> GetEffective();

        bool DeleteNormalizationRule(int normalizationRuleId);

        bool AreNormalizationRulesUpdated(ref object updateHandle);
    }
}
