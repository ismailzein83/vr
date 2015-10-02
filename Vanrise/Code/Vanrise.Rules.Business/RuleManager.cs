using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules.Entities;

namespace Vanrise.Rules.Business
{
    public class RuleManager
    {
        public StructuredRules StructureRules(List<BaseRule> rules, IEnumerable<BaseRuleSet> ruleSets)
        {
            StructuredRules structuredRules = new StructuredRules();
            BaseRuleSet current = null;

            List<BaseRuleSet> rulesSets2 = new List<BaseRuleSet>();
            rulesSets2.AddRange(ruleSets);
            rulesSets2.Add(new Entities.RuleSets.RuleSetByOthers());

            foreach (var ruleSet in rulesSets2)
            {
                for (int i = rules.Count - 1; i >= 0; i--)
                {
                    BaseRule rule = rules[i];
                    if (ruleSet.AddRuleIfMatched(rule))
                        rules.Remove(rule);
                }
                if (!ruleSet.IsEmpty())
                {
                    if (current != null)
                        current.NextRuleSet = ruleSet;
                    else
                        structuredRules.FirstRuleSet = ruleSet;
                    current = ruleSet;
                }
            }
            return structuredRules;
        }

        public BaseRule GetMostMatchedRule(StructuredRules rules, Object target)
        {
            return GetMostMatchedRule(rules.FirstRuleSet, target);
        }

        BaseRule GetMostMatchedRule(BaseRuleSet ruleSet, Object target)
        {
            if (ruleSet == null)
                return null;
            BaseRule rule = ruleSet.GetMatchedRule(target);
            if (rule != null)
                return rule;
            else
                return GetMostMatchedRule(ruleSet.NextRuleSet, target);
        }

    }
}
