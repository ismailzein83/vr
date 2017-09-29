using System;
using Vanrise.Common;
using Vanrise.Common.Business;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Sales.MainExtensions.PricingTemplateRate;

namespace TOne.WhS.Sales.MainExtensions.RateCalculation
{
    public class PricingTemplateCalculationMethod : RateCalculationMethod
    {
        #region Properties
        public int PricingTemplateId { get; set; }

        #endregion

        public override Guid ConfigId { get { throw new NotImplementedException(); } }

        public override void CalculateRate(IRateCalculationMethodContext context)
        {
            context.ThrowIfNull("context");
            context.ZoneItem.ThrowIfNull("context.ZoneItem", PricingTemplateId);

            PricingTemplate pricingTemplate = new PricingTemplate();
            pricingTemplate.ThrowIfNull("pricingTemplate", PricingTemplateId);
            pricingTemplate.Settinngs.ThrowIfNull("pricingTemplate.Settinngs", PricingTemplateId);
            pricingTemplate.Settinngs.Rules.ThrowIfNull(" pricingTemplate.Settinngs.Rules", PricingTemplateId);

            PricingTemplateRule matchingRule = GetPricingTemplateRule(context.ZoneItem.CountryId, context.ZoneItem.ZoneId, pricingTemplate);
            if (matchingRule == null)
                return;

            DateTime now = DateTime.Now;

            decimal? convertedRate;
            RatePricingTemplate ratePricingTemplate = GetRatePricingTemplate(context.ZoneItem, context.GetCostCalculationMethodIndex, matchingRule, pricingTemplate.Settinngs.CurrencyId, now, out convertedRate);
            if (ratePricingTemplate == null || !convertedRate.HasValue)
                return;

            ApplyMarginRatePricingTemplateContext applyMarginRatePricingTemplateContext = new ApplyMarginRatePricingTemplateContext() { Rate = convertedRate.Value };
            decimal convertedRateWithMargin = ratePricingTemplate.ApplyMargin(applyMarginRatePricingTemplateContext);

            context.Rate = new CurrencyExchangeRateManager().ConvertValueToCurrency(convertedRateWithMargin, pricingTemplate.Settinngs.CurrencyId, context.ZoneItem.TargetCurrencyId, now);
        }

        #region Private Methods
        private RatePricingTemplate GetRatePricingTemplate(ZoneItem zoneItem, Func<Guid, int?> getCostCalculationMethodIndex, PricingTemplateRule matchingRule,
            int pricingTemplateCurrencyId, DateTime effectiveDate, out decimal? convertedRate)
        {
            RatePricingTemplate ratePricingTemplate = null;
            convertedRate = null;

            CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();

            MarginRateCalculationContext marginRateCalculationContext = new MarginRateCalculationContext(getCostCalculationMethodIndex) { ZoneItem = zoneItem };
            foreach (RatePricingTemplate rateTemplate in matchingRule.Rates)
            {
                rateTemplate.MarginRateCalculation.ThrowIfNull("ratePricingTemplate.MarginRateCalculation", PricingTemplateId);

                decimal? rate = rateTemplate.MarginRateCalculation.GetRate(marginRateCalculationContext);
                if (rate.HasValue)
                {
                    decimal convertedRateTemplate = currencyExchangeRateManager.ConvertValueToCurrency(rate.Value, zoneItem.TargetCurrencyId, pricingTemplateCurrencyId, effectiveDate);
                    RatePricingTemplateContext ratePricingTemplateContext = new RatePricingTemplateContext() { ConvertedRate = convertedRateTemplate };
                    if (rateTemplate.IsRateMatching(ratePricingTemplateContext))
                    {
                        ratePricingTemplate = rateTemplate;
                        convertedRate = convertedRateTemplate;
                        break;
                    }
                }
            }
            return ratePricingTemplate;
        }

        private PricingTemplateRule GetPricingTemplateRule(int countryId, long saleZoneId, PricingTemplate pricingTemplate)
        {
            PricingTemplateRuleContext pricingTemplateRuleContext = new PricingTemplateRuleContext() { CountryId = countryId, SaleZoneId = saleZoneId };

            PricingTemplateRule matchingRule = null;
            foreach (PricingTemplateRule rule in pricingTemplate.Settinngs.Rules)
            {
                if (rule.IsZoneMatching(pricingTemplateRuleContext))
                {
                    matchingRule = rule;
                    matchingRule.Rates.ThrowIfNull("matchingRule.Rates", PricingTemplateId);
                    break;
                }
            }
            return matchingRule;
        }

        #endregion
    }
}