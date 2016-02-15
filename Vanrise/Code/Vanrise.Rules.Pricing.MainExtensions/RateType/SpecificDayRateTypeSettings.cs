using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing.MainExtensions.RateType
{
    public class SpecificDayRateTypeSettings : PricingRuleRateTypeItemSettings
    {
        public DateTime Date { get; set; }

        protected override bool Evaluate(IPricingRuleRateTypeItemContext context)
        {
            return context.TargetTime.HasValue && context.TargetTime.Value.Date == this.Date.Date;
        }

        public override string GetDescription()
        {
            return (Date != null) ? String.Format("Date: {0}", Date.ToString()) : null;
        }
    }
}
