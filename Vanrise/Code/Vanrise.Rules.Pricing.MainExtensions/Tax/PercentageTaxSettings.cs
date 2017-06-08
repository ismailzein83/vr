using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Vanrise.Rules.Pricing.MainExtensions.Tax
{
    public class PercentageTaxSettings : PricingRuleTaxActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("8C340085-E102-4504-A49B-329480CC7605"); }
        }
        public Decimal AmountToSkip { get; set; }
        public Decimal TaxPercentage { get; set; }

        protected override void Execute(IPricingRuleTaxActionContext context)
        {
            DateTime effectiveOn = context.TargetTime.HasValue ? context.TargetTime.Value : DateTime.Now;
            CurrencyExchangeRateManager currencyExchangeManager = new CurrencyExchangeRateManager();
            decimal? convertedFromAmount = null;
            decimal? convertedToAmount = null;
            if (FromAmount.HasValue)
                convertedFromAmount = currencyExchangeManager.ConvertValueToCurrency(FromAmount.Value, context.AmountCurrencyId, context.RuleCurrencyId, effectiveOn);
            if (ToAmount.HasValue)
                convertedToAmount = currencyExchangeManager.ConvertValueToCurrency(ToAmount.Value, context.AmountCurrencyId, context.RuleCurrencyId, effectiveOn);

            if (convertedFromAmount.HasValue && context.Amount < convertedFromAmount.Value)
                return;
            if (convertedToAmount.HasValue && context.Amount >= convertedToAmount.Value)
                return;
            CalculateAmount(context);
        }
        private void CalculateAmount(IPricingRuleTaxActionContext context)
        {
            if (context.Amount > this.AmountToSkip)
            {
                context.Percentage = this.TaxPercentage;
                context.TaxAmount = (this.TaxPercentage * (context.Amount - this.AmountToSkip) / 100);
            }else
            {
                context.TaxAmount = 0;
            }
           
        }
        public override string GetDescription()
        {
            StringBuilder description = new StringBuilder();
            if(this.FromAmount.HasValue)
            description.Append(String.Format("From Amount: {0}; ", FromAmount));
            if(this.ToAmount.HasValue)
             description.Append(String.Format("To Amount: {0}; ", ToAmount));
            description.Append(String.Format("Tax Percentage: {0}", TaxPercentage));
            return description.ToString();
        }
    }
}
