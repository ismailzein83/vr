using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Rules.RuleStructureBehaviors
{
    public abstract class RuleStructureBehaviorByKey<T> : BaseRuleStructureBehavior
    {
        VRDictionary<T, RuleNode> _ruleNodesByKey = new VRDictionary<T, RuleNode>();
        public override IEnumerable<RuleNode> StructureRules(IEnumerable<IVRRule> rules, out List<IVRRule> notMatchedRules)
        {
            notMatchedRules = new List<IVRRule>();
            foreach (var rule in rules)
            {
                IEnumerable<T> keys;
                GetKeysFromRule(rule, out keys);
                if (keys != null)
                {
                    foreach (var key in keys)
                    {
                        RuleNode matchNode = _ruleNodesByKey.GetOrCreateItem(key, () => new RuleNode { Rules = new List<IVRRule>() });
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

        protected abstract void GetKeysFromRule(IVRRule rule, out IEnumerable<T> keys);

        protected abstract bool TryGetKeyFromTarget(object target, out T key);
    }
}
