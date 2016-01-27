using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules.Pricing;

namespace Vanrise.GenericData.Pricing
{
    public class RateTypeRuleManager : Vanrise.GenericData.Business.GenericRuleManager<RateTypeRule>
    {
        public void ApplyRateTypeRule(IPricingRuleRateTypeContext context, int ruleDefinitionId, GenericRuleTarget target)
        {
            bool rateTypeApplied = false;
            if (context.RatesByRateType != null && context.RatesByRateType.Count > 0)
            {
                var rateTypePricingRule = GetMatchRule(ruleDefinitionId, target);
                if (rateTypePricingRule != null)
                {
                    rateTypePricingRule.Settings.ApplyRateTypeRule(context);
                    rateTypeApplied = true;
                }
            }
            if (!rateTypeApplied)
                context.EffectiveRate = context.NormalRate;
        }
    }
}
