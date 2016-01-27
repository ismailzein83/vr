using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Pricing
{
    public class RateTypeRule : Vanrise.GenericData.Entities.GenericRule
    {
        public Vanrise.Rules.Pricing.PricingRuleRateTypeSettings Settings { get; set; }
    }
}
