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
    public class RateTypeRuleManager : Vanrise.GenericData.Business.GenericRuleManager<RateTypeRule>
    {
        public void ApplyRateTypeRule(IPricingRuleRateTypeContext context, int ruleDefinitionId, GenericRuleTarget target)
        {
            this.ApplyRateTypeRule(context, () => base.GetMatchRule(ruleDefinitionId, target), target);
        }

        public void ApplyRateTypeRule(IPricingRuleRateTypeContext context, RuleTree ruleTree, GenericRuleTarget target)
        {
            this.ApplyRateTypeRule(context, () => ruleTree.GetMatchRule(target) as RateTypeRule, target);
        }

        void ApplyRateTypeRule(IPricingRuleRateTypeContext context, Func<RateTypeRule> getMatchRule, GenericRuleTarget target)
        {
            bool rateTypeApplied = false;
            if (context.RatesByRateType != null && context.RatesByRateType.Count > 0 && !target.IsEffectiveInFuture)
            {
                var rateTypePricingRule = getMatchRule();
                if (rateTypePricingRule != null)
                {
                    rateTypePricingRule.Settings.ApplyRateTypeRule(context);
                    context.Rule = rateTypePricingRule;
                    rateTypeApplied = true;
                }
            }
            if (!rateTypeApplied)
                context.EffectiveRate = context.NormalRate;
        }
    }
}
