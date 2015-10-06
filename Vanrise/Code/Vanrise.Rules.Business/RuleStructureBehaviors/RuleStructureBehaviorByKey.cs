using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Rules.Business.RuleStructureBehaviors
{
    public abstract class RuleStructureBehaviorByKey<T> : RuleStructureBehavior
    {
        Dictionary<T, RuleNode> _ruleNodesByKey = new Dictionary<T, RuleNode>();
        public override IEnumerable<RuleNode> StructureRules(IEnumerable<Entities.BaseRule> rules, out List<Entities.BaseRule> notMatchedRules)
        {
            notMatchedRules = new List<Entities.BaseRule>();
            foreach(var rule in rules)
            {
                IEnumerable<T> keys;
                GetKeysFromRule(rule, out keys);
                if (keys != null && keys.Count() > 0)
                {
                    foreach (var key in keys)
                    {
                        RuleNode matchNode = _ruleNodesByKey.GetOrCreateItem(key, () => new RuleNode { Rules = new List<Entities.BaseRule>() });
                        matchNode.Rules.Add(rule);
                    }
                }
                else
                    notMatchedRules.Add(rule);
            }
            return _ruleNodesByKey.Values;
        }

        public override RuleNode GetMatchedNode(object target)
        {
            T key;
            if(TryGetKeyFromTarget(target, out key))
            {
                RuleNode ruleNode;
                if (_ruleNodesByKey.TryGetValue(key, out ruleNode))
                    return ruleNode;
            }
            return null;
        }

        protected abstract void GetKeysFromRule(Entities.BaseRule rule, out IEnumerable<T> keys);

        protected abstract bool TryGetKeyFromTarget(object target, out T key);
    }
}
