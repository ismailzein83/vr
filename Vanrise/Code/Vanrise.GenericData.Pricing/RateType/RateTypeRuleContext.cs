using System;
using System.Collections.Generic;
using Vanrise.Rules;
using Vanrise.Rules.Pricing;

namespace Vanrise.GenericData.Pricing
{
    public class RateTypeRuleContext : IPricingRuleRateTypeContext
    {
        public DateTime? TargetTime { get; set; }

        public int? RateTypeId { get; set; }

        public BaseRule Rule { get; set; }

        public List<int> RateTypes { get; set; }
    }
}
