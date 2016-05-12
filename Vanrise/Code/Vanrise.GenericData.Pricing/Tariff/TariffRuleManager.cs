using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules.Pricing;

namespace Vanrise.GenericData.Pricing
{
    public class TariffRuleManager : Vanrise.GenericData.Business.GenericRuleManager<TariffRule>
    {
        public void ApplyTariffRule(IPricingRuleTariffContext context, int ruleDefinitionId, GenericRuleTarget target)
        {
            var tariffPricingRule = GetMatchRule(ruleDefinitionId, target);
            if (tariffPricingRule != null)
                tariffPricingRule.Settings.ApplyTariffRule(context);
            else
            {
                var effectiveRate = context.Rate;
                context.EffectiveRate = effectiveRate;
                if (context.DurationInSeconds != null)
                {
                    context.EffectiveDurationInSeconds = context.DurationInSeconds;
                    context.TotalAmount = effectiveRate * (context.DurationInSeconds.Value / 60);
                }
            }
        }
    }
}
