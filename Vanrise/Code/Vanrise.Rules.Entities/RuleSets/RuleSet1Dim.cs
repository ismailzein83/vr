using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Rules.Entities.RuleSets
{
    public abstract class RuleSet1Dim<T> : BaseRuleSet
    {
        Dictionary<T, List<BaseRule>> _rulesByKey = new Dictionary<T, List<BaseRule>>();
       
        public abstract void GetKeysFromRule(BaseRule r, out IEnumerable<T> keys);

        public abstract void GetKeysFromTarget(object target, out T key);

        public override BaseRule GetMatchedRule(object target)
        {
            T key;
            GetKeysFromTarget(target, out key);
            List<BaseRule> matchRules;
            if (_rulesByKey.TryGetValue(key, out matchRules))
            {
                foreach (var r in matchRules)
                {
                    if (r.EvaluateAdvancedConditions(target))
                        return r;
                }
            }
            return null;
        }
        public override bool IsEmpty()
        {
            return _rulesByKey.Count == 0;
        }

        public override bool AddRuleIfMatched(BaseRule rule)
        {
            IEnumerable<T> keys;
            GetKeysFromRule(rule, out keys);
            if (keys != null)
            {
                foreach (var key in keys)
                {
                    List<BaseRule> matchRules = _rulesByKey.GetOrCreateItem(key);
                    matchRules.Add(rule);
                    return true;
                }
            }
            return false;
        }
    }
}
