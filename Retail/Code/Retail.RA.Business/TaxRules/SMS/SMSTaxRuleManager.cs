using System;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;

namespace Retail.RA.Business
{
    public class SMSTaxRuleManager : Vanrise.GenericData.Business.GenericRuleManager<SMSTaxRule>
    {
        public void ApplyTaxRule(ISMSTaxRuleContext context, Guid ruleDefinitionId, GenericRuleTarget target)
        {
            this.ApplyTaxRule(context, () => GetMatchRule(ruleDefinitionId, target), target);
        }

        public void ApplyTaxRule(ISMSTaxRuleContext context, RuleTree ruleTree, GenericRuleTarget target)
        {
            this.ApplyTaxRule(context, () => ruleTree.GetMatchRule(target) as SMSTaxRule, target);
        }

        void ApplyTaxRule(ISMSTaxRuleContext context, Func<SMSTaxRule> getMatchRule, GenericRuleTarget target)
        {
            var taxRule = getMatchRule();
            if (taxRule != null)
            {
                taxRule.Settings.ApplySMSTaxRule(context);
                context.Rule = taxRule;
            }
        }
    }
}