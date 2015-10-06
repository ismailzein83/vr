using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Rules.Business.RuleStructureBehaviors
{
    public abstract class RuleStructureBehaviorByPrefix : RuleStructureBehavior
    {
        Dictionary<string, RuleNode> _ruleNodesByPrefixes = new Dictionary<string, RuleNode>();
        int _minPrefixLength = int.MaxValue;
        int _maxPrefixLength = 0;

        public override IEnumerable<RuleNode> StructureRules(IEnumerable<Entities.BaseRule> rules, out List<Entities.BaseRule> notMatchRules)
        {
            notMatchRules = new List<Entities.BaseRule>();
            foreach (var rule in rules)
            {
                IEnumerable<string> prefixes;
                GetPrefixesFromRule(rule, out prefixes);
                if (prefixes != null && prefixes.Count() > 0)
                {
                    foreach (var prefix in prefixes)
                    {
                        int prefixLength = prefix.Length;
                        if (prefixLength < _minPrefixLength)
                            _minPrefixLength = prefixLength;
                        if (prefixLength > _maxPrefixLength)
                            _maxPrefixLength = prefixLength;
                        RuleNode matchNode = _ruleNodesByPrefixes.GetOrCreateItem(prefix, () => new RuleNode { Rules = new List<Entities.BaseRule>() });
                        matchNode.Rules.Add(rule);
                    }
                }
                else
                    notMatchRules.Add(rule);
            }
            return _ruleNodesByPrefixes.Values;
        }

        public override RuleNode GetMatchedNode(object target)
        {
            string valueToCompare;
            if (TryGetValueToCompareFromTarget(target, out valueToCompare) && valueToCompare != null)
            {
                RuleNode ruleNode;
                string prefix = valueToCompare.Substring(0, Math.Min(_maxPrefixLength, valueToCompare.Length));
                while (prefix.Length >= _minPrefixLength)
                {
                    if (_ruleNodesByPrefixes.TryGetValue(prefix, out ruleNode))
                        return ruleNode;
                    prefix = prefix.Substring(0, prefix.Length - 1);
                }
            }
            return null;
        }

        protected abstract void GetPrefixesFromRule(Entities.BaseRule rule, out IEnumerable<string> prefixes);

        protected abstract bool TryGetValueToCompareFromTarget(object target, out string value);
    }
}
