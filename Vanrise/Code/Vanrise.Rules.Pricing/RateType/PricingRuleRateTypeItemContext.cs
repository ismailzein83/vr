using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public class PricingRuleRateTypeItemContext : IPricingRuleRateTypeItemContext
    {
        public decimal NormalRate
        {
            get;
            set;
        }

        public Dictionary<int, decimal> RatesByRateType
        {
            get;
            set;
        }

        public DateTime? TargetTime
        {
            get;
            set;
        }
    }
}
