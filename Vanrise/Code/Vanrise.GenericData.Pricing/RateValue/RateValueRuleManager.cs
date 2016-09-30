using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;
using Vanrise.Rules.Pricing;

namespace Vanrise.GenericData.Pricing
{
    public class RateValueRuleManager : Vanrise.GenericData.Business.GenericRuleManager<RateValueRule>
    {
        public void ApplyRateValueRule(IPricingRuleRateValueContext context, Guid ruleDefinitionId, GenericRuleTarget target)
        {
            this.ApplyRateValueRule(context, () => GetMatchRule(ruleDefinitionId, target), target);
        }

        public void ApplyRateValueRule(IPricingRuleRateValueContext context, RuleTree ruleTree, GenericRuleTarget target)
        {
            this.ApplyRateValueRule(context, () => ruleTree.GetMatchRule(target) as RateValueRule, target);
        }

        void ApplyRateValueRule(IPricingRuleRateValueContext context, Func<RateValueRule> getMatchRule, GenericRuleTarget target)
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
