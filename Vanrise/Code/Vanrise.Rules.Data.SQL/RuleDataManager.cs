using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Rules.Entities;

namespace Vanrise.Rules.Data.SQL
{
    public class RuleDataManager : BaseSQLDataManager, IRuleDataManager
    {
        #region Properties/Ctor

        public RuleDataManager()
            : base(GetConnectionStringName("RulesDBConnStringKey", "RulesDBConnString"))
        {

        }

        #endregion

        #region Public Methods

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

        public bool AddRuleAndRuleChanged(Entities.Rule rule, ActionType actionType, out int ruleId)
        {
            object insertedId;
            int recordesEffected = ExecuteNonQuerySP("rules.sp_Rule_InsertRuleAndRuleChanged", out insertedId, rule.TypeId, rule.RuleDetails, rule.BED, rule.EED, actionType);
            ruleId = (recordesEffected > 0) ? (int)insertedId : -1;
            return (recordesEffected > 0);
        }

        public bool UpdateRuleAndRuleChanged(Entities.Rule rule, ActionType actionType, string initialRule, string additionalInformation)
        {
            int recordesEffected = ExecuteNonQuerySP("rules.sp_Rule_UpdateRuleAndRuleChanged", rule.RuleId, rule.TypeId, rule.RuleDetails, rule.BED, rule.EED, actionType, initialRule, additionalInformation);
            return (recordesEffected > 0);
        }

        public bool DeleteRule(int ruleId)
        {
            int recordesEffected = ExecuteNonQuerySP("rules.sp_Rule_Delete", ruleId);
            return (recordesEffected > 0);
        }

        public IEnumerable<Entities.Rule> GetRulesByType(int ruleTypeId)
        {
            return GetItemsSP("rules.sp_Rule_GetByType", RuleMapper, ruleTypeId);
        }

        public bool AreRulesUpdated(int ruleTypeId, ref object updateHandle)
        {
            return base.IsDataUpdated("rules.[Rule]", "TypeID", ruleTypeId, ref updateHandle);
        }

        public int GetRuleTypeId(string ruleType)
        {
            return (int)ExecuteScalarSP("rules.sp_RuleType_InsertIfNotExistsAndGetID", ruleType);
        }

        public RuleChanged GetRuleChanged(int ruleId, int ruleTypeId)
        {
            return GetItemSP("rules.sp_RuleChanged_GetByRuleAndType", RuleChangedMapper, ruleId, ruleTypeId);
        }

        public List<RuleChanged> GetRulesChanged(int ruleTypeId)
        {
            return GetItemsSP("rules.sp_RuleChanged_GetByType", RuleChangedMapper, ruleTypeId);
        }

        public RuleChanged GetRuleChangedForProcessing(int ruleId, int ruleTypeId)
        {
            return GetItemSP("rules.sp_RuleChangedForProcessing_GetByRuleAndType", RuleChangedMapper, ruleId, ruleTypeId);
        }

        public List<RuleChanged> GetRulesChangedForProcessing(int ruleTypeId)
        {
            return GetItemsSP("rules.sp_RuleChangedForProcessing_GetByType", RuleChangedMapper, ruleTypeId);
        }

        public RuleChanged FillAndGetRuleChangedForProcessing(int ruleId, int ruleTypeId)
        {
            return GetItemSP("rules.sp_RuleChangedForProcessing_FillAndGetByRuleAndType", RuleChangedMapper, ruleId, ruleTypeId);
        }

        public List<RuleChanged> FillAndGetRulesChangedForProcessing(int ruleTypeId)
        {
            return GetItemsSP("rules.sp_RuleChangedForProcessing_FillAndGetByType", RuleChangedMapper, ruleTypeId);
        }

        public void DeleteRuleChangedForProcessing(int ruleId, int ruleTypeId)
        {
            ExecuteNonQuerySP("rules.sp_RuleChangedForProcessing_DeleteByRuleAndType", ruleId, ruleTypeId);
        }

        public void DeleteRulesChangedForProcessing(int ruleTypeId)
        {
            ExecuteNonQuerySP("rules.sp_RuleChangedForProcessing_DeleteByType", ruleTypeId);
        }
        #endregion

        #region Private Methods

        private Entities.Rule RuleMapper(IDataReader reader)
        {
            Entities.Rule instance = new Entities.Rule
            {
                RuleId = (int)reader["ID"],
                TypeId = GetReaderValue<int>(reader, "TypeID"),
                RuleDetails = reader["RuleDetails"] as string,
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
            };
            return instance;
        }

        private RuleChanged RuleChangedMapper(IDataReader reader)
        {
            RuleChanged ruleChanged = new RuleChanged
            {
                RuleChangedId = GetReaderValue<int>(reader, "ID"),
                RuleId = GetReaderValue<int>(reader, "RuleId"),
                RuleTypeId = GetReaderValue<int>(reader, "RuleTypeId"),
                ActionType = GetReaderValue<ActionType>(reader, "ActionType"),
                InitialRule = reader["InitialRule"] as string,
                AdditionalInformation = reader["AdditionalInformation"] as string,
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
            };
            return ruleChanged;
        }

        #endregion
    }
}