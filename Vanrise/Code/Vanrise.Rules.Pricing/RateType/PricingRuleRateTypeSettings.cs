using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public class PricingRuleRateTypeSettings
    {
        public List<PricingRuleRateTypeItemSettings> Items { get; set; }

        public void ApplyRateTypeRule(IPricingRuleRateTypeContext context)
        {
           if (context.RatesByRateType != null && context.RatesByRateType.Count > 0)
            {
                bool isRateFound = false;

                PricingRuleRateTypeItemContext itemContext = new PricingRuleRateTypeItemContext
                {
                    NormalRate = context.NormalRate,
                    RatesByRateType = context.RatesByRateType,
                    TargetTime = context.TargetTime
                };

                foreach (var rateTypeItem in this.Items)
                {
                    if (rateTypeItem.Evaluate(itemContext))
                    {
                        Decimal rateToUse;
                        if (context.RatesByRateType.TryGetValue(rateTypeItem.RateTypeId, out rateToUse))
                        {
                            isRateFound = true;
                            context.EffectiveRate = rateToUse;
                            context.RateTypeId = rateTypeItem.RateTypeId;
                            break;
                        }
                    }
                }
                if (!isRateFound)
                    context.EffectiveRate = context.NormalRate;
            }
            else
               context.EffectiveRate = context.NormalRate;
        }
    }
}
