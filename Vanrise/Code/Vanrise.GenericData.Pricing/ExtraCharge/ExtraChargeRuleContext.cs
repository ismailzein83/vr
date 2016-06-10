using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules;
using Vanrise.Rules.Pricing;

namespace Vanrise.GenericData.Pricing
{
    public class ExtraChargeRuleContext : IPricingRuleExtraChargeContext
    {
        public DateTime? TargetTime { get; set; }

        public Decimal Rate { get; set; }

        public Decimal ExtraChargeValue { get; set; }

        public BaseRule Rule { get; set; }
    }
}
