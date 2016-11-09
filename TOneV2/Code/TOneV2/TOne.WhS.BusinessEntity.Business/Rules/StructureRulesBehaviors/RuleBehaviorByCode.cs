using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors
{
    public class RuleBehaviorByCode : Vanrise.Rules.BaseRuleStructureBehavior
    {
        RuleBehaviorByCodeAsKey _ruleBehaviorByCodeAsKey = new RuleBehaviorByCodeAsKey();
        RuleBehaviorByCodeAsPrefix _ruleBehaviorByCodeAsPrefix = new RuleBehaviorByCodeAsPrefix();

        public override IEnumerable<Vanrise.Rules.RuleNode> StructureRules(IEnumerable<Vanrise.Rules.IVRRule> rules, out List<Vanrise.Rules.IVRRule> notMatchRules)
        {
            List<Vanrise.Rules.IVRRule> notMatchRules1;
            var nodes1 = _ruleBehaviorByCodeAsKey.StructureRules(rules, out notMatchRules1);
            List<Vanrise.Rules.IVRRule> notMatchRules2;
            var nodes2 = _ruleBehaviorByCodeAsPrefix.StructureRules(rules, out notMatchRules2);

            List<Vanrise.Rules.RuleNode> allNodes = new List<Vanrise.Rules.RuleNode>();
            if (nodes1 != null)
                allNodes.AddRange(nodes1);
            if (nodes2 != null)
                allNodes.AddRange(nodes2);

            if (notMatchRules1 == null || notMatchRules2 == null)
                notMatchRules = null;
            else
                notMatchRules = notMatchRules1.Intersect(notMatchRules2).ToList();

            return allNodes;
        }

        public override List<Vanrise.Rules.RuleNode> GetMatchedNodes(object target)
        {
            List<Vanrise.Rules.RuleNode> matchedRules = new List<Vanrise.Rules.RuleNode>();
            
            List<Vanrise.Rules.RuleNode> matchedRulesByKey = _ruleBehaviorByCodeAsKey.GetMatchedNodes(target);
            if (matchedRulesByKey != null)
                matchedRules.AddRange(matchedRulesByKey);

            List<Vanrise.Rules.RuleNode> matchedRulesByPrefix = _ruleBehaviorByCodeAsPrefix.GetMatchedNodes(target);
            if (matchedRulesByPrefix != null)
                matchedRules.AddRange(matchedRulesByPrefix);

            return matchedRules.Count > 0 ? matchedRules : null;
        }

        public override Vanrise.Rules.BaseRuleStructureBehavior CreateNewBehaviorObject()
        {
            return new RuleBehaviorByCode();
        }

        #region Private Classes

        private class RuleBehaviorByCodeAsKey : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<string>
        {
            protected override void GetKeysFromRule(Vanrise.Rules.IVRRule rule, out IEnumerable<string> keys)
            {
                keys = null;
                IRuleCodeCriteria ruleCodeCriteria = rule as IRuleCodeCriteria;
                IEnumerable<CodeCriteria> codeCriterias = ruleCodeCriteria.CodeCriterias;
                if (codeCriterias != null)
                    keys = codeCriterias.Select(code => code.Code);
            }

            protected override bool TryGetKeyFromTarget(object target, out string key)
            {
                IRuleCodeTarget ruleCodeTarget = target as IRuleCodeTarget;
                if (ruleCodeTarget.Code != null)
                {
                    key = ruleCodeTarget.Code;
                    return true;
                }
                else
                {
                    key = null;
                    return false;
                }
            }

            public override Vanrise.Rules.BaseRuleStructureBehavior CreateNewBehaviorObject()
            {
                return new RuleBehaviorByCodeAsKey();
            }
        }

        private class RuleBehaviorByCodeAsPrefix : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByPrefix
        {
            protected override void GetPrefixesFromRule(Vanrise.Rules.IVRRule rule, out IEnumerable<string> prefixes)
            {
                prefixes = null;
                IRuleCodeCriteria ruleCodeCriteria = rule as IRuleCodeCriteria;
                IEnumerable<CodeCriteria> codeCriterias = ruleCodeCriteria.CodeCriterias;
                if (codeCriterias != null)
                    prefixes = codeCriterias.Where(code => code.WithSubCodes).Select(code => code.Code);
            }

            protected override bool TryGetValueToCompareFromTarget(object target, out string value)
            {
                IRuleCodeTarget ruleCodeTarget = target as IRuleCodeTarget;
                if (ruleCodeTarget.Code != null)
                {
                    value = ruleCodeTarget.Code;
                    return true;
                }
                else
                {
                    value = null;
                    return false;
                }
            }

            public override Vanrise.Rules.BaseRuleStructureBehavior CreateNewBehaviorObject()
            {
                return new RuleBehaviorByCodeAsPrefix();
            }
        }

        #endregion
    }
}
