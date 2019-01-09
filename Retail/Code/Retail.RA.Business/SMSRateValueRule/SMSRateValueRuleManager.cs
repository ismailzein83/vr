using System;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;

namespace Retail.RA.Business
{
    public class SMSRateValueRuleManager : Vanrise.GenericData.Business.GenericRuleManager<SMSRateValueRule>
    {
        public void ApplyRateValueRule(ISMSRateValueRuleContext context, Guid ruleDefinitionId, GenericRuleTarget target)
        {
            this.ApplyRateValueRule(context, () => GetMatchRule(ruleDefinitionId, target), target);
        }

        public void ApplyRateValueRule(ISMSRateValueRuleContext context, RuleTree ruleTree, GenericRuleTarget target)
        {
            this.ApplyRateValueRule(context, () => ruleTree.GetMatchRule(target) as SMSRateValueRule, target);
        }

        void ApplyRateValueRule(ISMSRateValueRuleContext context, Func<SMSRateValueRule> getMatchRule, GenericRuleTarget target)
        {
            var rateValueRule = getMatchRule();
            if (rateValueRule != null)
            {
                rateValueRule.Settings.ApplyRateValueRule(context);
                context.Rule = rateValueRule;
            }
        }
    }
}
