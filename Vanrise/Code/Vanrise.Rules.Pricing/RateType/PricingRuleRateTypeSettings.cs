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
            if (context.RateTypes == null || context.RateTypes.Count == 0)

                return;

            PricingRuleRateTypeItemContext itemContext = new PricingRuleRateTypeItemContext
            {
                RateTypes = context.RateTypes,
                TargetTime = context.TargetTime
            };

            foreach (var rateTypeItem in this.Items)
            {
                if (!rateTypeItem.Evaluate(itemContext))
                    continue;

                if (!context.RateTypes.Contains(rateTypeItem.RateTypeId))
                    continue;

                context.RateTypeId = rateTypeItem.RateTypeId;
                break;
            }
        }
    }
}
