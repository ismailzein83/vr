using System;
using System.Collections.Generic;

namespace TOne.WhS.Sales.Entities
{
    public class RatePricingTemplate
    {
        public decimal FromRate { get; set; }

        public decimal ToRate { get; set; }

        public decimal? MinRate { get; set; }

        public decimal? MaxRate { get; set; }

        public decimal? Margin { get; set; }

        public decimal? MarginPercentage { get; set; }

        public MarginRateCalculation MarginRateCalculation { get; set; }

        public bool IsRateMatching(IRatePricingTemplateContext context)
        {
            if (context.ConvertedRate >= FromRate && context.ConvertedRate < ToRate)
                return true;

            return false;
        }

        public decimal ApplyMargin(IApplyMarginRatePricingTemplateContext context)
        {
            decimal rate = context.Rate;
            if (this.Margin.HasValue)
                rate += this.Margin.Value;

            if (this.MarginPercentage.HasValue)
                rate += (this.MarginPercentage.Value * rate) / 100;

            if (this.MinRate.HasValue && rate < this.MinRate.Value)
                rate = this.MinRate.Value;

            if (this.MaxRate.HasValue && rate > this.MaxRate.Value)
                rate = this.MaxRate.Value;

            return rate;
        }
    }

    public interface IRatePricingTemplateContext
    {
        decimal ConvertedRate { get; }
    }

    public class RatePricingTemplateContext : IRatePricingTemplateContext
    {
        public decimal ConvertedRate { get; set; }
    }

    public interface IApplyMarginRatePricingTemplateContext
    {
        decimal Rate { get; }
    }

    public class ApplyMarginRatePricingTemplateContext : IApplyMarginRatePricingTemplateContext
    {
        public decimal Rate { get; set; }
    }
}