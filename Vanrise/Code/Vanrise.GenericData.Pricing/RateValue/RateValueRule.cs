using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Pricing
{
    public class RateValueRule : Vanrise.GenericData.Entities.GenericRule
    {
        public Vanrise.Rules.Pricing.PricingRuleRateValueSettings Settings { get; set; }
    }
}
