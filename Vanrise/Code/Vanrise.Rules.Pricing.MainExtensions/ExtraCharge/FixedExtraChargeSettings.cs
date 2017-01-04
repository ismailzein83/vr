using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Vanrise.Rules.Pricing.MainExtensions.ExtraCharge
{
    public class FixedExtraChargeSettings : PricingRuleExtraChargeActionSettings
    {
        public override Guid ConfigId { get { return  new Guid("9b441d3f-1a1d-4060-8bb1-740cef377e0d"); } }

        public Decimal FromRate { get; set; }

        public Decimal ToRate { get; set; }

        public Decimal ExtraAmount { get; set; }

        protected override void Execute(IPricingRuleExtraChargeActionContext context)
        {
            DateTime effectiveOn = context.TargetTime.HasValue ? context.TargetTime.Value : DateTime.Now;
            CurrencyExchangeRateManager currencyExchangeManager = new CurrencyExchangeRateManager();
            decimal convertedFromRate;
            decimal convertedToRate;
            decimal convertedExtraAmount;

            if (context.DestinationCurrencyId.HasValue)
            {
                convertedFromRate = currencyExchangeManager.ConvertValueToCurrency(FromRate, context.SourceCurrencyId, context.DestinationCurrencyId.Value, effectiveOn);
                convertedToRate = currencyExchangeManager.ConvertValueToCurrency(ToRate, context.SourceCurrencyId, context.DestinationCurrencyId.Value, effectiveOn);
                convertedExtraAmount = currencyExchangeManager.ConvertValueToCurrency(ExtraAmount, context.SourceCurrencyId, context.DestinationCurrencyId.Value, effectiveOn);
            }
            else
            {
                convertedFromRate = FromRate;
                convertedToRate = ToRate;
                convertedExtraAmount = ExtraAmount;
            }

            if (context.Rate >= convertedFromRate && context.Rate < convertedToRate)
            {
                context.Rate += convertedExtraAmount;
                context.IsExtraChargeApplied = true;
            }
        }

        public override string GetDescription()
        {
            StringBuilder description = new StringBuilder();
            description.Append(String.Format("From Rate: {0}; ", FromRate));
            description.Append(String.Format("To Rate: {0}; ", ToRate));
            description.Append(String.Format("Extra Amount: {0}", ExtraAmount));
            return description.ToString();
        }
    }
}
