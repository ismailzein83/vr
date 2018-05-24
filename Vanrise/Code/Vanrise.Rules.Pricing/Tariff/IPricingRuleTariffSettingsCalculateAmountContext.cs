using System;

namespace Vanrise.Rules.Pricing
{
    public interface IPricingRuleTariffSettingsCalculateAmountContext
    {
        decimal EffectiveRate { get; }
        decimal DurationInSeconds { get; }
        decimal Amount { set; }
    }
}