using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Rules.Entities.RuleSets
{
    public abstract class RuleSet3Dim<T, Q, R> : BaseRuleSet
    {
        Dictionary<string, List<BaseRule>> _rulesByKey = new Dictionary<string, List<BaseRule>>();

        public override BaseRule GetMatchedRule(object target)
        {
            T key1;
            Q key2;
            R key3;
            GetKeysFromTarget(target, out key1, out key2, out key3);

            List<BaseRule> matchRules;
            string key = String.Format("{0}_{1}_{2}", key1, key2, key3);
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

        public abstract void GetKeysFromRule(BaseRule r, out IEnumerable<T> keys1, out IEnumerable<Q> keys2, out IEnumerable<R> keys3);

        public abstract void GetKeysFromTarget(object target, out T key1, out Q key2, out R key3);

        public override bool IsEmpty()
        {
            return _rulesByKey.Count == 0;
        }

        public override bool AddRuleIfMatched(BaseRule rule)
        {
            IEnumerable<T> keys1;
            IEnumerable<Q> keys2;
            IEnumerable<R> keys3;
            GetKeysFromRule(rule, out keys1, out keys2, out keys3);
            if (keys1 != null && keys2 != null && keys3 != null)
            {
                foreach (var key1 in keys1)
                {
                    foreach (var key2 in keys2)
                    {
                        foreach (var key3 in keys3)
                        {
                            string key = String.Format("{0}_{1}_{2}", key1, key2, key3);
                            List<BaseRule> matchRules = _rulesByKey.GetOrCreateItem(key);
                            matchRules.Add(rule);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
