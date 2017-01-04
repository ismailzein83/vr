using System;

namespace Vanrise.Rules.Pricing
{
    public class PricingRuleExtraChargeActionContext : IPricingRuleExtraChargeActionContext
    {
        public DateTime? TargetTime { get; set; }

        public Decimal Rate { get; set; }

        public int? DestinationCurrencyId { get; set; }

        public int SourceCurrencyId { get; set; }

        public bool IsExtraChargeApplied { get; set; }
    }
}