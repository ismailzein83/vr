using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public class PricingRuleExtraChargeActionContext : IPricingRuleExtraChargeActionContext
    {
        public DateTime? TargetTime { get; set; }

        public Decimal Rate { get; set; }
    }
}
