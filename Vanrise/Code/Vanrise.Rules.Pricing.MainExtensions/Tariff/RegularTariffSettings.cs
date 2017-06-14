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

            DateTime currencyEffectiveOn = context.TargetTime.HasValue ? context.TargetTime.Value : DateTime.Now;

            CurrencyExchangeRateManager currencyExchangeManager = new CurrencyExchangeRateManager();
            decimal convertedFirstPeriodRate;
            decimal convertedCallFee;

            if (context.DestinationCurrencyId.HasValue)
            {
                convertedFirstPeriodRate = currencyExchangeManager.ConvertValueToCurrency(firstPeriodRateToUse, context.SourceCurrencyId, context.DestinationCurrencyId.Value, currencyEffectiveOn);
                convertedCallFee = currencyExchangeManager.ConvertValueToCurrency(CallFee, context.SourceCurrencyId, context.DestinationCurrencyId.Value, currencyEffectiveOn);
            }
            else
            {
                convertedFirstPeriodRate = firstPeriodRateToUse;
                convertedCallFee = CallFee;
            }

            decimal extraChargeValue = 0;
            decimal totalAmount = 0;

            Decimal? accountedDuration = context.DurationInSeconds;
            if (FirstPeriod > 0)
            {
                //Decimal firstPeriodRate = (convertedFirstPeriodRate > 0) ? (Decimal)convertedFirstPeriodRate : context.Rate;

                if (accountedDuration.HasValue)
                {
                    accountedDuration -= (Decimal)FirstPeriod;
                    accountedDuration = Math.Max(0, accountedDuration.Value);
                    totalAmount = convertedFirstPeriodRate;
                }
            }

            if (FractionUnit > 0)
            {
                FractionUnit = (byte)FractionUnit;
                if (accountedDuration.HasValue)
                    accountedDuration = Math.Ceiling(accountedDuration.Value / FractionUnit) * FractionUnit;
            }

            if (accountedDuration.HasValue)
            {
                decimal normalisedDuration = accountedDuration.Value / PricingUnit;
                totalAmount += normalisedDuration * context.Rate;
                extraChargeValue = normalisedDuration * context.ExtraChargeRate;
            }

            context.EffectiveRate = 60 * context.Rate / PricingUnit;
            context.EffectiveDurationInSeconds = accountedDuration.HasValue ? accountedDuration.Value + FirstPeriod : FirstPeriod;

            totalAmount += convertedCallFee;

            context.TotalAmount = decimal.Round(totalAmount, 8);
            context.ExtraChargeValue = decimal.Round(extraChargeValue, 8);
        }

        public override string GetDescription()
        {
            StringBuilder description = new StringBuilder();
            description.Append(String.Format("Call Fee: {0}; ", CallFee));
            description.Append(String.Format("First Period: {0}; ", FirstPeriod));
            description.Append(String.Format("First Period Rate: {0}; ", FirstPeriodRate));
            description.Append(String.Format("Fraction Unit: {0}", FractionUnit));
            return description.ToString();
        }
    }
}
