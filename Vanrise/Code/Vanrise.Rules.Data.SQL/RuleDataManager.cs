using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Rules.Data.SQL
{
    public class RuleDataManager : BaseSQLDataManager, IRuleDataManager
    {
        public RuleDataManager()
            : base(GetConnectionStringName("RulesDBConnStringKey", "RulesDBConnString"))
        {

        }

        public bool AddRule(Entities.Rule rule, out int ruleId)
        {
            object insertedId;
            int recordesEffected = ExecuteNonQuerySP("rules.sp_Rule_Insert", out insertedId, rule.TypeId, rule.RuleDetails, rule.BED, rule.EED);
            ruleId = (recordesEffected > 0) ? (int)insertedId : -1;
            return (recordesEffected > 0);
        }

        public bool UpdateRule(Entities.Rule ruleEntity)
        {
            int recordesEffected = ExecuteNonQuerySP("rules.sp_Rule_Update", ruleEntity.RuleId, ruleEntity.TypeId, ruleEntity.RuleDetails, ruleEntity.BED, ruleEntity.EED);
            return (recordesEffected > 0);
        }

        public bool DeleteRule(int ruleId)
        {
            int recordesEffected = ExecuteNonQuerySP("rules.sp_Rule_Delete", ruleId);
            return (recordesEffected > 0);
        }

        public IEnumerable<Entities.Rule> GetRulesByType(int ruleTypeId)
        {
            return GetItemsSP("rules.sp_Rule_GetByType",RuleMapper, ruleTypeId);
        }

        public bool AreRulesUpdated(int ruleTypeId, ref object updateHandle)
        {
            return base.IsDataUpdated("rules.[Rule]", "TypeID", ruleTypeId, ref updateHandle);
        }

        private Entities.Rule RuleMapper(IDataReader reader)
        {
            Entities.Rule instance = new Entities.Rule
            {
                RuleId = (int)reader["ID"],
                TypeId = GetReaderValue<int>(reader, "TypeID"),
                RuleDetails = reader["RuleDetails"] as string,
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED=  GetReaderValue<DateTime?>(reader, "EED"),
            };
            return instance;
        }


        public int GetRuleTypeId(string ruleType)
        {
            return (int)ExecuteScalarSP("rules.sp_RuleType_InsertIfNotExistsAndGetID", ruleType);
        }
    }
}
