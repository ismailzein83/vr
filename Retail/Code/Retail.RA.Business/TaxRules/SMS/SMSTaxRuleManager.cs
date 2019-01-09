using System;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;

namespace Retail.RA.Business
{
    public class SMSTaxRuleManager : Vanrise.GenericData.Business.GenericRuleManager<SMSTaxRule>
    {
        public void ApplySMSTaxRule(ISMSTaxRuleContext context, Guid ruleDefinitionId, GenericRuleTarget target)
        {
            this.ApplySMSTaxRule(context, () => GetMatchRule(ruleDefinitionId, target), target);
        }

        public void ApplySMSTaxRule(ISMSTaxRuleContext context, RuleTree ruleTree, GenericRuleTarget target)
        {
            this.ApplySMSTaxRule(context, () => ruleTree.GetMatchRule(target) as SMSTaxRule, target);
        }

        void ApplySMSTaxRule(ISMSTaxRuleContext context, Func<SMSTaxRule> getMatchRule, GenericRuleTarget target)
        {
            var smsTaxRule = getMatchRule();
            if (smsTaxRule != null)
            {
                smsTaxRule.Settings.ApplySMSTaxRule(context);
                context.Rule = smsTaxRule;
            }
        }
    }
}