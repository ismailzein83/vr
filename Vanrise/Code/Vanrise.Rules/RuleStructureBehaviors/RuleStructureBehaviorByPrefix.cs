﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Rules.RuleStructureBehaviors
{
    public abstract class RuleStructureBehaviorByPrefix : BaseRuleStructureBehavior
    {
        Dictionary<string, RuleNode> _ruleNodesByPrefixes = new Dictionary<string, RuleNode>();
        int _minPrefixLength = int.MaxValue;
        int _maxPrefixLength = 0;
        

        public override IEnumerable<RuleNode> StructureRules(IEnumerable<BaseRule> rules, out List<BaseRule> notMatchRules)
        {
            int priority = 0;
            int keyLength = 0;
            List<BaseRule> baseRules;

            notMatchRules = new List<BaseRule>();
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
                        RuleNode matchNode = _ruleNodesByPrefixes.GetOrCreateItem(prefix, () => new RuleNode { Rules = new List<BaseRule>(), Priorities = new Dictionary<BaseRule,int>() });
                        matchNode.Rules.Add(rule);
                        matchNode.Priorities.Add(rule, 0);
                    }
                }
                else
                    notMatchRules.Add(rule);
            }
            _ruleNodesByPrefixes = _ruleNodesByPrefixes.OrderByDescending(itm => itm.Key.Length).ToDictionary(key => key.Key, value => value.Value);

            //add rules having parent prefixes
            foreach (var ruleNodesByPrefix in _ruleNodesByPrefixes)
            {
                if (ruleNodesByPrefix.Key.Length != keyLength)
                    priority++;

                keyLength = ruleNodesByPrefix.Key.Length;

                foreach (var rule in _ruleNodesByPrefixes.Where(itm => itm.Key != ruleNodesByPrefix.Key && ruleNodesByPrefix.Key.StartsWith(itm.Key)).SelectMany(itm => itm.Value.Rules))
                {
                    if (!ruleNodesByPrefix.Value.Rules.Contains(rule))
                    {
                        ruleNodesByPrefix.Value.Rules.Add(rule);
                        ruleNodesByPrefix.Value.Priorities.Add(rule, priority);
                    }
                }
            }
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

        protected abstract void GetPrefixesFromRule(BaseRule rule, out IEnumerable<string> prefixes);

        protected abstract bool TryGetValueToCompareFromTarget(object target, out string value);
    }
}
