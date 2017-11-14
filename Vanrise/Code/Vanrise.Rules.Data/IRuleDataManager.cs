using System.Collections.Generic;
using Vanrise.Rules.Entities;

namespace Vanrise.Rules.Data
{
    public interface IRuleDataManager : IDataManager
    {
        bool AddRule(Rule rule, out int ruleId);

        bool UpdateRule(Rule ruleEntity);

        bool AddRuleAndRuleChanged(Rule rule, ActionType actionType, out int ruleId);

        bool UpdateRuleAndRuleChanged(Rule rule, ActionType actionType, string initialRule, string additionalInformation);

        bool DeleteRule(int ruleId);

        IEnumerable<Rule> GetRulesByType(int ruleTypeId);

        bool AreRulesUpdated(int ruleTypeId, ref object updateHandle);

        int GetRuleTypeId(string ruleType);

        RuleChanged GetRuleChanged(int ruleId, int ruleTypeId);

        List<RuleChanged> GetRulesChanged(int ruleTypeId);

        void DeleteRuleChanged(int ruleId, int ruleTypeId);

        void DeleteRulesChanged(int ruleTypeId);
    }
}
