using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Entities.RuleSets
{
    public class RuleSetByOthers : BaseRuleSet
    {
        List<BaseRule> _rules = new List<BaseRule>();
        public override BaseRule GetMatchedRule(object target)
        {
            foreach (var r in _rules)
            {
                if (r.EvaluateAdvancedConditions(target))
                    return r;
            }
            return null;
        }

        public override bool AddRuleIfMatched(BaseRule rule)
        {
            _rules.Add(rule);
            return true;
        }

        public override bool IsEmpty()
        {
            return _rules.Count > 0;
        }
    }
}
