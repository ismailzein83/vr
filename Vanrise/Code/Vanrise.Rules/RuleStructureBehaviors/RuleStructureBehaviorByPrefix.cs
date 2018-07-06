using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Rules.RuleStructureBehaviors
{
    public abstract class RuleStructureBehaviorByPrefix : BaseRuleStructureBehavior
    {
        VRDictionary<string, RuleNode> _ruleNodesByPrefixes = new VRDictionary<string, RuleNode>();
        int _minPrefixLength = int.MaxValue;
        int _maxPrefixLength = 0;


        public override IEnumerable<RuleNode> StructureRules(IEnumerable<IVRRule> rules, out List<IVRRule> notMatchRules)
        {
            notMatchRules = new List<IVRRule>();
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
                        RuleNode matchNode = _ruleNodesByPrefixes.GetOrCreateItem(prefix, () => new RuleNode { Rules = new List<IVRRule>(), Priorities = new Dictionary<IVRRule, int>() });
                        matchNode.Rules.Add(rule);
                        matchNode.Priorities.Add(rule, 0);
                    }
                }
                else
                    notMatchRules.Add(rule);
            }
            var orderedRuleNodesByPrefixes = _ruleNodesByPrefixes.OrderByDescending(itm => itm.Key.Length).ToList();

            ////add rules having parent prefixes
            for (int i = 0; i < orderedRuleNodesByPrefixes.Count; i++)
            {
                int priority = 0;
                var ruleNodesByPrefix = orderedRuleNodesByPrefixes[i];

                int matchingNodeLength = 0;
                for (int j = i + 1; j < orderedRuleNodesByPrefixes.Count; j++)
                {
                    var matchingNode = orderedRuleNodesByPrefixes[j];
                    if (matchingNode.Key != ruleNodesByPrefix.Key && ruleNodesByPrefix.Key.StartsWith(matchingNode.Key))
                    {
                        if (matchingNode.Key.Length != matchingNodeLength)
                            priority++;

                        matchingNodeLength = matchingNode.Key.Length;

                        foreach (var rule in matchingNode.Value.Rules)
                        {
                            if (!ruleNodesByPrefix.Value.Rules.Contains(rule))
                            {
                                ruleNodesByPrefix.Value.Rules.Add(rule);
                                ruleNodesByPrefix.Value.Priorities.Add(rule, priority);
                            }
                        }
                    }
                }
            }


            ////add rules having parent prefixes
            //foreach (var ruleNodesByPrefix in orderedRuleNodesByPrefixes)
            //{
            //    if (ruleNodesByPrefix.Key.Length != keyLength)
            //        priority++;

            //    keyLength = ruleNodesByPrefix.Key.Length;

            //    var matchingNodes = orderedRuleNodesByPrefixes.Where(itm => itm.Key != ruleNodesByPrefix.Key && ruleNodesByPrefix.Key.StartsWith(itm.Key)).OrderByDescending(itm => itm.Key.Length);//.SelectMany(itm => itm.Value.Rules);
            //    int matchingNodeLength = 0;

            //    foreach (var matchingNode in matchingNodes)
            //    {
            //        if (matchingNode.Key.Length != matchingNodeLength)
            //            priority++;

            //        matchingNodeLength = matchingNode.Key.Length;

            //        foreach (var rule in matchingNode.Value.Rules)
            //        {
            //            if (!ruleNodesByPrefix.Value.Rules.Contains(rule))
            //            {
            //                ruleNodesByPrefix.Value.Rules.Add(rule);
            //                ruleNodesByPrefix.Value.Priorities.Add(rule, priority);
            //            }
            //        }
            //    }
            //}
            return _ruleNodesByPrefixes.Values;
        }

        public override List<RuleNode> GetMatchedNodes(object target)
        {
            string valueToCompare;
            if (TryGetValueToCompareFromTarget(target, out valueToCompare) && valueToCompare != null)
            {
                RuleNode ruleNode;
                string prefix = valueToCompare.Substring(0, Math.Min(_maxPrefixLength, valueToCompare.Length));
                while (prefix.Length >= _minPrefixLength)
                {
                    if (_ruleNodesByPrefixes.TryGetValue(prefix, out ruleNode))
                        return new List<RuleNode>() { ruleNode };
                    prefix = prefix.Substring(0, prefix.Length - 1);
                }
            }
            return null;
        }

        protected abstract void GetPrefixesFromRule(IVRRule rule, out IEnumerable<string> prefixes);

        protected abstract bool TryGetValueToCompareFromTarget(object target, out string value);
    }
}
