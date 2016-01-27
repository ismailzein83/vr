using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public abstract class PricingRuleRateTypeItemSettings
    {
        public int ConfigId { get; set; }

        public int RateTypeId { get; set; }

        internal protected abstract bool Evaluate(IPricingRuleRateTypeItemContext context);
    }
}
