using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public class PricingRuleRateTypeItemContext : IPricingRuleRateTypeItemContext
    {
        public DateTime? TargetTime { get; set; }

        public List<int> RateTypes { get; set; }
    }
}
