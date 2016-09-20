using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Vanrise.Rules.Pricing.MainExtensions.ExtraCharge
{
    public class PercentageExtraChargeSettings : PricingRuleExtraChargeActionSettings
    {
        public override Guid ConfigId { get { return  new Guid("9387367e-4bbc-4f8a-958f-af27effc7ec4"); } }
        public Decimal FromRate { get; set; }

        public Decimal ToRate { get; set; }

        public Decimal ExtraPercentage { get; set; }

        protected override void Execute(IPricingRuleExtraChargeActionContext context)
        {
            DateTime effectiveOn = context.TargetTime.HasValue ? context.TargetTime.Value : DateTime.Now;
            CurrencyExchangeRateManager currencyExchangeManager = new CurrencyExchangeRateManager();
            decimal convertedFromRate;
            decimal convertedToRate;

            if (context.DestinationCurrencyId.HasValue)
            {
                convertedFromRate = currencyExchangeManager.ConvertValueToCurrency(FromRate, context.SourceCurrencyId, context.DestinationCurrencyId.Value, effectiveOn);
                convertedToRate = currencyExchangeManager.ConvertValueToCurrency(ToRate, context.SourceCurrencyId, context.DestinationCurrencyId.Value, effectiveOn);
            }
            else
            {
                convertedFromRate = FromRate;
                convertedToRate = ToRate;
            }


            if (context.Rate >= convertedFromRate && context.Rate < convertedToRate)
                context.Rate += (this.ExtraPercentage * context.Rate / 100);
        }

        public override string GetDescription()
        {
            StringBuilder description = new StringBuilder();
            description.Append(String.Format("From Rate: {0}; ", FromRate));
            description.Append(String.Format("To Rate: {0}; ", ToRate));
            description.Append(String.Format("Extra Percentage: {0}", ExtraPercentage));
            return description.ToString();
        }
    }
}
