using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.PricingRules.RateTypeSettings
{
    public class SpecificDayRateTypeSettings : PricingRuleRateTypeItemSettings
    {
        public DateTime Date { get; set; }

        public override bool Evaluate(IPricingRuleRateTypeItemContext context, PricingRuleRateTypeTarget target)
        {
            return target.EffectiveOn.HasValue && target.EffectiveOn.Value == this.Date;
        }
    }
}
