using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Rules.RuleStructureBehaviors
{
    public abstract class RuleStructureBehaviorByKey<T> : BaseRuleStructureBehavior
    {
        Dictionary<T, RuleNode> _ruleNodesByKey = new Dictionary<T, RuleNode>();
        public override IEnumerable<RuleNode> StructureRules(IEnumerable<BaseRule> rules, out List<BaseRule> notMatchedRules)
        {
            notMatchedRules = new List<BaseRule>();
            foreach (var rule in rules)
            {
                IEnumerable<T> keys;
                GetKeysFromRule(rule, out keys);
                if (keys != null && keys.Count() > 0)
                {
                    foreach (var key in keys)
                    {
                        RuleNode matchNode = _ruleNodesByKey.GetOrCreateItem(key, () => new RuleNode { Rules = new List<BaseRule>() });
                        matchNode.Rules.Add(rule);
                    }
                }
                else
                    notMatchedRules.Add(rule);
            }
            return _ruleNodesByKey.Values;
        }

        public override List<RuleNode> GetMatchedNodes(object target)
        {
            T key;
            if (TryGetKeyFromTarget(target, out key))
            {
                RuleNode ruleNode;
                if (_ruleNodesByKey.TryGetValue(key, out ruleNode))
                {
                    return new List<RuleNode>() { ruleNode };
                }
            }
            return null;
        }

        protected abstract void GetKeysFromRule(BaseRule rule, out IEnumerable<T> keys);

        protected abstract bool TryGetKeyFromTarget(object target, out T key);
    }
}
