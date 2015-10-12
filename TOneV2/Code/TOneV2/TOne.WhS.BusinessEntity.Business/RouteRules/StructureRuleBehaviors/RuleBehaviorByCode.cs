using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business.RouteRules.StructureRuleBehaviors
{
    public class RuleBehaviorByCode : Vanrise.Rules.BaseRuleStructureBehavior
    {
        RuleBehaviorByCodeAsKey _ruleBehaviorByCodeAsKey = new RuleBehaviorByCodeAsKey();
        RuleBehaviorByCodeAsPrefix _ruleBehaviorByCodeAsPrefix = new RuleBehaviorByCodeAsPrefix();

        public override IEnumerable<Vanrise.Rules.RuleNode> StructureRules(IEnumerable<Vanrise.Rules.BaseRule> rules, out List<Vanrise.Rules.BaseRule> notMatchRules)
        {            
            List<Vanrise.Rules.BaseRule> notMatchRules1;
            var nodes1 = _ruleBehaviorByCodeAsKey.StructureRules(rules, out notMatchRules1);
            List<Vanrise.Rules.BaseRule> notMatchRules2;
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

        public override Vanrise.Rules.RuleNode GetMatchedNode(object target)
        {
            Vanrise.Rules.RuleNode matchedRule = null;
            matchedRule = _ruleBehaviorByCodeAsKey.GetMatchedNode(target);
            if (matchedRule == null)
                matchedRule = _ruleBehaviorByCodeAsPrefix.GetMatchedNode(target);
            return matchedRule;
        }

        #region Private Classes

        private class RuleBehaviorByCodeAsKey : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<string>
        {
            protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<string> keys)
            {
                keys = null;
                IRuleCodeCriteria ruleCodeCriteria = rule as IRuleCodeCriteria;
                if (ruleCodeCriteria.CodeCriteriaGroupSettings != null)
                {
                    CodeManager codeManager = new CodeManager();
                    List<CodeCriteria> codeCriterias = codeManager.GetCodeCriterias(ruleCodeCriteria.CodeCriteriaGroupSettings.ConfigId, ruleCodeCriteria.CodeCriteriaGroupSettings);
                    if (codeCriterias != null)
                        keys = codeCriterias.Where(code => !code.WithSubCodes).Select(code => code.Code);
                }
            }

            protected override bool TryGetKeyFromTarget(object target, out string key)
            {
                RouteIdentifier routeIdentifier = target as RouteIdentifier;
                key = routeIdentifier.Code;
                return true;
            }
        }

        private class RuleBehaviorByCodeAsPrefix : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByPrefix
        {
            protected override void GetPrefixesFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<string> prefixes)
            {
                prefixes = null;
                IRuleCodeCriteria ruleCodeCriteria = rule as IRuleCodeCriteria;
                if (ruleCodeCriteria.CodeCriteriaGroupSettings != null)
                {
                    CodeManager codeManager = new CodeManager();
                    List<CodeCriteria> codeCriterias = codeManager.GetCodeCriterias(ruleCodeCriteria.CodeCriteriaGroupSettings.ConfigId, ruleCodeCriteria.CodeCriteriaGroupSettings);
                    if(codeCriterias != null)
                        prefixes = codeCriterias.Where(code => code.WithSubCodes).Select(code => code.Code);
                }                    
            }

            protected override bool TryGetValueToCompareFromTarget(object target, out string value)
            {
                RouteIdentifier routeIdentifier = target as RouteIdentifier;
                value = routeIdentifier.Code;
                return true;
            }
        }

        #endregion
    }
}
