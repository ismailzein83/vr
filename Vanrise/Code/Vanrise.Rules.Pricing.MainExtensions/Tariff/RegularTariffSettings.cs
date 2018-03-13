using System;
using System.Text;
using Vanrise.Common.Business;

namespace Vanrise.Rules.Pricing.MainExtensions.Tariff
{
    public enum FirstPeriodRateType { FixedRate = 0, EffectiveRate = 1 }
    public class RegularTariffSettings : PricingRuleTariffSettings
    {
        public override Guid ConfigId { get { return new Guid("35acc9c2-0675-4347-ba3e-a81025c1be12"); } }

        public Decimal CallFee { get; set; }

        public int FirstPeriod { get; set; }

        public FirstPeriodRateType FirstPeriodRateType { get; set; }

        public Decimal? FirstPeriodRate { get; set; }

        public int FractionUnit { get; set; }

        public int PricingUnit { get; set; }

        protected override void Execute(IPricingRuleTariffContext context)
        {
            if (FractionUnit < 0)
                throw new ArgumentException(string.Format("Invalid FractionUnit: {0}", FractionUnit));

            if (PricingUnit <= 0)
                throw new ArgumentException(string.Format("Invalid PricingUnit: {0}", PricingUnit));

            context.EffectiveRate = 60 * context.Rate / PricingUnit;

            if (!context.DurationInSeconds.HasValue)
                return;

            decimal remainingDurationToPrice = context.DurationInSeconds.Value;

            decimal firstPeriodRateToUse;
            switch (FirstPeriodRateType)
            {
                case Tariff.FirstPeriodRateType.EffectiveRate:
                    firstPeriodRateToUse = context.Rate;
                    break;

                case Tariff.FirstPeriodRateType.FixedRate:
                    if (!FirstPeriodRate.HasValue)
                        throw new NullReferenceException("FirstPeriodRate");

                    firstPeriodRateToUse = FirstPeriodRate.Value;
                    break;

                default: throw new NotSupportedException(string.Format("FirstPeriodRateType has invalid value: {0}", FirstPeriodRateType));
            }

            CurrencyExchangeRateManager currencyExchangeManager = new CurrencyExchangeRateManager();
            decimal convertedFirstPeriodRate;
            decimal convertedCallFee;

            DateTime currencyEffectiveOn = context.TargetTime.HasValue ? context.TargetTime.Value : DateTime.Now;

            if (context.DestinationCurrencyId.HasValue)
            {
                switch (this.FirstPeriodRateType)
                {
                    case Tariff.FirstPeriodRateType.EffectiveRate: convertedFirstPeriodRate = firstPeriodRateToUse; break;
                    default: convertedFirstPeriodRate = currencyExchangeManager.ConvertValueToCurrency(firstPeriodRateToUse, context.SourceCurrencyId, context.DestinationCurrencyId.Value, currencyEffectiveOn); break;
                }
                convertedCallFee = currencyExchangeManager.ConvertValueToCurrency(CallFee, context.SourceCurrencyId, context.DestinationCurrencyId.Value, currencyEffectiveOn);
            }
            else
            {
                convertedFirstPeriodRate = firstPeriodRateToUse;
                convertedCallFee = CallFee;
            }

            decimal totalAmount = 0;
            decimal extraChargeValue = 0;
            decimal chargedFirstPeriodDuration = 0;

            if (FirstPeriod > 0)
            {
                remainingDurationToPrice -= (Decimal)FirstPeriod;
                remainingDurationToPrice = Math.Max(0, remainingDurationToPrice);

                switch (FirstPeriodRateType)
                {
                    case Tariff.FirstPeriodRateType.EffectiveRate:
                        chargedFirstPeriodDuration = (FractionUnit > 0) ? (Math.Ceiling((decimal)FirstPeriod / FractionUnit) * FractionUnit) : FirstPeriod;
                        totalAmount = convertedFirstPeriodRate * chargedFirstPeriodDuration / PricingUnit;
                        break;

                    case Tariff.FirstPeriodRateType.FixedRate:
                        chargedFirstPeriodDuration = FirstPeriod;
                        totalAmount = convertedFirstPeriodRate;
                        break;
                }
            }

            if (FractionUnit > 0)
                remainingDurationToPrice = Math.Ceiling(remainingDurationToPrice / FractionUnit) * FractionUnit;

            decimal normalisedDuration = remainingDurationToPrice / PricingUnit;
            totalAmount += normalisedDuration * context.Rate;
            extraChargeValue = normalisedDuration * context.ExtraChargeRate;

            context.EffectiveDurationInSeconds = chargedFirstPeriodDuration + remainingDurationToPrice;

            totalAmount += convertedCallFee;

            context.TotalAmount = decimal.Round(totalAmount, 8);
            context.ExtraChargeValue = decimal.Round(extraChargeValue, 8);
        }

        public override string GetDescription()
        {
            StringBuilder description = new StringBuilder();
            description.Append(String.Format("Call Fee: {0}; ", CallFee));
            description.Append(String.Format("First Period: {0}; ", FirstPeriod));


            string firstPeriodRateDesc;
            switch (FirstPeriodRateType)
            {
                case Tariff.FirstPeriodRateType.EffectiveRate:
                    firstPeriodRateDesc = "Effective Rate";
                    break;

                case Tariff.FirstPeriodRateType.FixedRate:
                    if (!FirstPeriodRate.HasValue)
                        throw new NullReferenceException("FirstPeriodRate");

                    firstPeriodRateDesc = FirstPeriodRate.Value.ToString();
                    break;
                default: throw new NotSupportedException(string.Format("FirstPeriodRateType has invalid value: {0}", FirstPeriodRateType));
            }


            description.Append(String.Format("First Period Rate: {0}; ", firstPeriodRateDesc));
            description.Append(String.Format("Fraction Unit: {0}; ", FractionUnit));
            description.Append(String.Format("Pricing Unit: {0}", PricingUnit));
            return description.ToString();
        }

        public override string GetPricingDescription()
        {
            var effectiveFractionUnit = FractionUnit == 0 ? 1 : FractionUnit;
            int firstFraction = FirstPeriod == 0 ? effectiveFractionUnit : FirstPeriod;
            return String.Format("{0}/{1}", firstFraction, effectiveFractionUnit);
        }
    }
}
