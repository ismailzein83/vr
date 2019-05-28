using System;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class MinMarginPercentageRateThreshold : RateThreshold
    {
        public override Guid ConfigId => new Guid("E78BD4E6-6D9C-4432-B564-5B5A1DD1EF73");
        public int Percentage { get; set; }

        public override void Execute(ISellingRuleContext context)
        {
            decimal ratePercentageValue = (context.CurrentRate * Percentage) / 100;
            decimal calculatedRate = context.CurrentRate - ratePercentageValue;

            context.ViolateRateRule = context.NewRate < calculatedRate;
        }

        public override string GetDescription()
        {
            return $"Min Percentage: {Percentage}%";
        }
    }
}
