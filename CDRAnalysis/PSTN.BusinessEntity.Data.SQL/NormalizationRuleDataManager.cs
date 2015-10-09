using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace PSTN.BusinessEntity.Data.SQL
{
    public class NormalizationRuleDataManager : Vanrise.Data.SQL.BaseSQLDataManager, INormalizationRuleDataManager
    {
        //private Dictionary<string, string> _mapper;

        public NormalizationRuleDataManager() : base("CDRDBConnectionString")
        {
            //_mapper = new Dictionary<string, string>();
            //_mapper.Add("NormalizationRuleId", "ID");
            //_mapper.Add("BeginEffectiveDate", "BED");
            //_mapper.Add("EndEffectiveDate", "EED");
        }

        public List<NormalizationRule> GetEffective()
        {
            return GetItemsSP("PSTN_BE.sp_NormalizationRule_GetEffective", NormalizationRuleMapper);
        }

        //public Vanrise.Entities.BigResult<NormalizationRule> GetFilteredNormalizationRules(Vanrise.Entities.DataRetrievalInput<NormalizationRuleQuery> input)
        //{
        //    return RetrieveData(input, (tempTableName) =>
        //    {
        //        ExecuteNonQuerySP("PSTN_BE.sp_NormalizationRule_CreateTempByFiltered", tempTableName, input.Query.EffectiveDate);

        //    }, (reader) => NormalizationRuleMapper(reader), _mapper);
        //}

        public List<NormalizationRule> GetNormalizationRules()
        {
            return GetItemsSP("PSTN_BE.sp_NormalizationRule_GetAll", NormalizationRuleMapper);
        }

        public NormalizationRule GetNormalizationRuleById(int normalizationRuleId)
        {
            return GetItemSP("PSTN_BE.sp_NormalizationRule_GetByID", NormalizationRuleMapper, normalizationRuleId);
        }

        public bool AddNormalizationRule(NormalizationRule normalizationRuleObj, out int insertedID)
        {
            object normalizationRuleId;

            string serializedCriteria = Vanrise.Common.Serializer.Serialize(normalizationRuleObj.Criteria);
            string serializedSettings = Vanrise.Common.Serializer.Serialize(normalizationRuleObj.Settings);

            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_NormalizationRule_Insert", out normalizationRuleId, serializedCriteria, serializedSettings, normalizationRuleObj.BeginEffectiveDate, normalizationRuleObj.EndEffectiveDate);

            insertedID = (recordsAffected > 0) ? (int)normalizationRuleId : -1;
            return (recordsAffected > 0);
        }

        public bool UpdateNormalizationRule(NormalizationRule normalizationRuleObj)
        {
            string serializedCriteria = Vanrise.Common.Serializer.Serialize(normalizationRuleObj.Criteria);
            string serializedSettings = Vanrise.Common.Serializer.Serialize(normalizationRuleObj.Settings);

            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_NormalizationRule_Update", normalizationRuleObj.NormalizationRuleId, serializedCriteria, serializedSettings, normalizationRuleObj.BeginEffectiveDate, normalizationRuleObj.EndEffectiveDate);
            return (recordsAffected > 0);
        }

        public bool DeleteNormalizationRule(int normalizationRuleId)
        {
            int recordsEffected = ExecuteNonQuerySP("PSTN_BE.sp_NormalizationRule_Delete", normalizationRuleId);
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

            normalizationRule.NormalizationRuleId = (int)reader["ID"];
            normalizationRule.Criteria = Vanrise.Common.Serializer.Deserialize<NormalizationRuleCriteria>(reader["Criteria"] as string);
            normalizationRule.Settings = Vanrise.Common.Serializer.Deserialize<NormalizationRuleSettings>(reader["Settings"] as string);
            normalizationRule.Description = GetReaderValue<string>(reader, "Description");
            normalizationRule.BeginEffectiveDate = (DateTime)reader["BED"];
            normalizationRule.EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED");

            return normalizationRule;
        }

        #endregion
    }
}
