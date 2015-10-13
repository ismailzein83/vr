using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules.Entities;

namespace Vanrise.Rules.Data
{
    public interface IRuleDataManager : IDataManager
    {
        bool AddRule(Rule rule, out int ruleId);

        bool UpdateRule(Rule ruleEntity);

        bool DeleteRule(int ruleId);

        IEnumerable<Rule> GetRulesByType(int ruleTypeId);
        
        bool AreRulesUpdated(int ruleTypeId, ref object updateHandle);

        int GetRuleTypeId(string ruleType);
    }
}
