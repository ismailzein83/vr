using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace PSTN.BusinessEntity.Data.SQL
{
    public class NormalizationRuleDataManager : Vanrise.Data.SQL.BaseSQLDataManager, INormalizationRuleDataManager
    {
        private Dictionary<string, string> _mapper;

        public NormalizationRuleDataManager() : base("CDRDBConnectionString")
        {
            _mapper = new Dictionary<string, string>();
            _mapper.Add("NormalizationRuleId", "ID");
            _mapper.Add("BeginEffectiveDate", "BED");
            _mapper.Add("EndEffectiveDate", "EED");
        }

        public Vanrise.Entities.BigResult<NormalizationRuleDetail> GetFilteredNormalizationRules(Vanrise.Entities.DataRetrievalInput<NormalizationRuleQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("PSTN_BE.sp_NormalizationRule_CreateTempByFiltered", tempTableName, input.Query.BeginEffectiveDate, input.Query.EndEffectiveDate);

            }, (reader) => NormalizationRuleDetailMapper(reader), _mapper);
        }

        public NormalizationRule GetNormalizationRuleByID(int normalizationRuleId)
        {
            return GetItemSP("PSTN_BE.sp_NormalizationRule_GetByID", NormalizationRuleMapper);
        }

        public bool AddNormalizationRule(NormalizationRule normalizationRuleObj, out int insertedID)
        {
            object normalizationRuleId;

            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_NormalizationRule_Insert", out normalizationRuleId, normalizationRuleObj.Criteria, normalizationRuleObj.Settings);

            insertedID = (recordsAffected > 0) ? (int)normalizationRuleId : -1;
            return (recordsAffected > 0);
        }

        #region Mappers

        private NormalizationRuleDetail NormalizationRuleDetailMapper(IDataReader reader)
        {
            NormalizationRuleDetail normalizationRuleDetail = new NormalizationRuleDetail();

            normalizationRuleDetail.NormalizationRuleId = (int)reader["ID"];
            normalizationRuleDetail.BeginEffectiveDate = (DateTime)reader["BED"];
            normalizationRuleDetail.EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED");
            
            return normalizationRuleDetail;
        }

        private NormalizationRule NormalizationRuleMapper(IDataReader reader)
        {
            NormalizationRule normalizationRule = new NormalizationRule();

            normalizationRule.NormalizationRuleId = (int)reader["ID"];
            normalizationRule.Criteria = Vanrise.Common.Serializer.Deserialize<NormalizationRuleCriteria>(reader["Criteria"] as string);
            normalizationRule.Settings = Vanrise.Common.Serializer.Deserialize<NormalizationRuleSettings>(reader["Settings"] as string);

            return normalizationRule;
        }

        #endregion
    }
}
