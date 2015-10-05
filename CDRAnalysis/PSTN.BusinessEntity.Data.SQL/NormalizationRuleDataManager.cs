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

            }, (reader) => NormalizationRuleMapper(reader), _mapper);
        }

        #region Mappers

        private NormalizationRuleDetail NormalizationRuleMapper(IDataReader reader)
        {
            NormalizationRuleDetail normalizationRuleDetail = new NormalizationRuleDetail();

            normalizationRuleDetail.NormalizationRuleId = (int)reader["ID"];
            normalizationRuleDetail.BeginEffectiveDate = (DateTime)reader["BED"];
            normalizationRuleDetail.EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED");
            
            return normalizationRuleDetail;
        }

        #endregion
    }
}
