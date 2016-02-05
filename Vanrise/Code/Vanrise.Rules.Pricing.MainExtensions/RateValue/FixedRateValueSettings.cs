using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing.MainExtensions.RateValue
{
    public class FixedRateValueSettings : PricingRuleRateValueSettings
    {
        public Decimal NormalRate { get; set; }

        public Dictionary<int, Decimal> RatesByRateType { get; set; }

        protected override void Execute(IPricingRuleRateValueContext context)
        {
            context.NormalRate = this.NormalRate;
            context.RatesByRateType = this.RatesByRateType;
        }
    }
}
