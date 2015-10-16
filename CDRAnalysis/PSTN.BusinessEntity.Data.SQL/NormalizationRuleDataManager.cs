using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace PSTN.BusinessEntity.Data.SQL
{
    public class NormalizationRuleDataManager : Vanrise.Data.SQL.BaseSQLDataManager, INormalizationRuleDataManager
    {

        public NormalizationRuleDataManager()
            : base("ConfigurationDBConnString")
        {
        }

        public List<NormalizationRule> GetEffective()
        {
            return GetItemsSP("PSTN_BE.sp_NormalizationRule_GetEffective", NormalizationRuleMapper);
        }

        public bool DeleteNormalizationRule(int normalizationRuleId)
        {
            int recordsEffected = ExecuteNonQuerySP("rules.sp_Rule_Delete", normalizationRuleId);
            return (recordsEffected > 0);
        }

        public bool AreNormalizationRulesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("PSTN_BE.NormalizationRule", ref updateHandle);
        }

        #region Mappers

        private NormalizationRule NormalizationRuleMapper(IDataReader reader)
        {
            NormalizationRule normalizationRule = new NormalizationRule();

            normalizationRule.RuleId = (int)reader["ID"];
            normalizationRule.Criteria = Vanrise.Common.Serializer.Deserialize<NormalizationRuleCriteria>(reader["Criteria"] as string);
            normalizationRule.Settings = Vanrise.Common.Serializer.Deserialize<NormalizationRuleSettings>(reader["Settings"] as string);
            normalizationRule.Description = GetReaderValue<string>(reader, "Description");
            normalizationRule.BeginEffectiveTime = (DateTime)reader["BED"];
            normalizationRule.EndEffectiveTime = GetReaderValue<DateTime?>(reader, "EED");

            return normalizationRule;
        }

        #endregion
    }
}
