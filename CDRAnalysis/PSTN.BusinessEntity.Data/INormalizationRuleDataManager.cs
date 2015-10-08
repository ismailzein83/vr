﻿using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface INormalizationRuleDataManager : IDataManager
    {
        List<NormalizationRule> GetEffective();
        Vanrise.Entities.BigResult<NormalizationRuleDetail> GetFilteredNormalizationRules(Vanrise.Entities.DataRetrievalInput<NormalizationRuleQuery> input);

        NormalizationRule GetNormalizationRuleByID(int normalizationRuleId);

        NormalizationRuleDetail GetNormalizationRuleDetailByID(int normalizationRuleId);

        bool AddNormalizationRule(NormalizationRule normalizationRuleObj, out int insertedID);

        bool UpdateNormalizationRule(NormalizationRule normalizationRuleObj);

        bool DeleteNormalizationRule(int normalizationRuleId);
    }
}
