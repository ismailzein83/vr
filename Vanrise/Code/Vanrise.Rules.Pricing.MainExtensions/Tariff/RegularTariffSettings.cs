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
        public Decimal CallFee { get; set; }

        public int FirstPeriod { get; set; }

        public Decimal FirstPeriodRate { get; set; }

        public int FractionUnit { get; set; }

        protected override void Execute(IPricingRuleTariffContext context)
        {
            CurrencyExchangeRateManager currencyExchangeManager = new CurrencyExchangeRateManager();
            decimal convertedFirstPeriodRate;
            decimal convertedCallFee;

            if (context.DestinationCurrencyId.HasValue)
            {
                convertedFirstPeriodRate = currencyExchangeManager.ConvertValueToCurrency(FirstPeriodRate, context.SourceCurrencyId, context.DestinationCurrencyId.Value, context.TargetTime.Value);
                convertedCallFee = currencyExchangeManager.ConvertValueToCurrency(CallFee, context.SourceCurrencyId, context.DestinationCurrencyId.Value, context.TargetTime.Value);
            }
            else
            {
                convertedFirstPeriodRate = FirstPeriodRate;
                convertedCallFee = CallFee;
            }

            int? currencyId = context.DestinationCurrencyId;
            decimal extraChargeValue = 0;
            Decimal? totalAmount = 0;
            Decimal? accountedDuration = context.DurationInSeconds;
            if (FirstPeriod > 0)
            {
                FirstPeriod = FirstPeriod;
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
                {
                    accountedDuration = Math.Ceiling(accountedDuration.Value / FractionUnit) * FractionUnit;
                    if (totalAmount.HasValue)
                        totalAmount += Math.Ceiling(accountedDuration.Value / FractionUnit) * context.Rate;// 60

                    extraChargeValue = Math.Ceiling(accountedDuration.Value / FractionUnit) * context.ExtraChargeRate;
                }
                //context.EffectiveRate = Math.Ceiling(FractionUnit * context.Rate / 60);
                context.EffectiveRate = 60 * context.Rate / FractionUnit;
                context.EffectiveDurationInSeconds = accountedDuration + FirstPeriod;
            }
            else
            {
                if (accountedDuration.HasValue && totalAmount.HasValue)
                {
                    totalAmount += Math.Ceiling((accountedDuration.Value * context.Rate) / 60);
                }
                context.EffectiveRate = context.Rate;
                context.EffectiveDurationInSeconds = accountedDuration;
            }
            
            if (totalAmount.HasValue)
                totalAmount += convertedCallFee;

            context.TotalAmount = totalAmount;
            context.ExtraChargeValue = extraChargeValue;
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
