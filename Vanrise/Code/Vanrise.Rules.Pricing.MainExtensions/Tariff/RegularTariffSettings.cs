using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Vanrise.Rules.Pricing.MainExtensions.Tariff
{
    public class RegularTariffSettings : PricingRuleTariffSettings
    {
        public override Guid ConfigId { get { return new Guid("35acc9c2-0675-4347-ba3e-a81025c1be12"); } }
        public Decimal CallFee { get; set; }

        public int FirstPeriod { get; set; }

        public Decimal FirstPeriodRate { get; set; }

        public int FractionUnit { get; set; }

        public int PricingUnit { get; set; }

        protected override void Execute(IPricingRuleTariffContext context)
        {
            if (FractionUnit < 0)
                throw new ArgumentException(string.Format("Invalid FractionUnit: {0}", FractionUnit));

            if (PricingUnit <= 0)
                throw new ArgumentException(string.Format("Invalid PricingUnit: {0}", PricingUnit));

            DateTime currencyEffectiveOn = context.TargetTime.HasValue ? context.TargetTime.Value : DateTime.Now;

            CurrencyExchangeRateManager currencyExchangeManager = new CurrencyExchangeRateManager();
            decimal convertedFirstPeriodRate;
            decimal convertedCallFee;

            if (context.DestinationCurrencyId.HasValue)
            {
                convertedFirstPeriodRate = currencyExchangeManager.ConvertValueToCurrency(FirstPeriodRate, context.SourceCurrencyId, context.DestinationCurrencyId.Value, currencyEffectiveOn);
                convertedCallFee = currencyExchangeManager.ConvertValueToCurrency(CallFee, context.SourceCurrencyId, context.DestinationCurrencyId.Value, currencyEffectiveOn);
            }
            else
            {
                convertedFirstPeriodRate = FirstPeriodRate;
                convertedCallFee = CallFee;
            }

            decimal extraChargeValue = 0;
            decimal totalAmount = 0;

            Decimal? accountedDuration = context.DurationInSeconds;
            if (FirstPeriod > 0)
            {
                Decimal firstPeriodRate = (convertedFirstPeriodRate > 0) ? (Decimal)convertedFirstPeriodRate : context.Rate;

                if (accountedDuration.HasValue)
                {
                    accountedDuration -= (Decimal)FirstPeriod;
                    accountedDuration = Math.Max(0, accountedDuration.Value);
                    totalAmount = firstPeriodRate;
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
