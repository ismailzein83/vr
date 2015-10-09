using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface INormalizationRuleDataManager : IDataManager
    {
        List<NormalizationRule> GetEffective();
        //Vanrise.Entities.BigResult<NormalizationRule> GetFilteredNormalizationRules(Vanrise.Entities.DataRetrievalInput<NormalizationRuleQuery> input);

        List<NormalizationRule> GetNormalizationRules();

        NormalizationRule GetNormalizationRuleByID(int normalizationRuleId);

        bool AddNormalizationRule(NormalizationRule normalizationRuleObj, out int insertedID);

        bool UpdateNormalizationRule(NormalizationRule normalizationRuleObj);

        bool DeleteNormalizationRule(int normalizationRuleId);

        bool AreNormalizationRulesUpdated(ref object updateHandle);
    }
}
