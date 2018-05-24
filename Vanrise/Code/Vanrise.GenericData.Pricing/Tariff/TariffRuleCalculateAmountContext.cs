using System;

namespace Vanrise.Rules.Pricing
{
    public class TariffRuleCalculateAmountContext : IPricingRuleTariffSettingsCalculateAmountContext
    {
        public decimal EffectiveRate { get; set; }
        public decimal DurationInSeconds { get; set; }
        public decimal Amount { get; set; }
    }
}