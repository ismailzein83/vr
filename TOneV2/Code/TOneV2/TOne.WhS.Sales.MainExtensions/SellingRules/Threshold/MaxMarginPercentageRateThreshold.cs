using System;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class MaxMarginPercentageRateThreshold : RateThreshold
    {
        public override Guid ConfigId => new Guid("2B794920-FE60-4186-80DE-1CB05512F338");
        public int Percentage { get; set; }

        public override void Execute(ISellingRuleContext context)
        {
            decimal ratePercentageValue = (context.CurrentRate * Percentage) / 100;
            decimal calculatedRate = context.CurrentRate + ratePercentageValue;

            context.ViolateRateRule = context.NewRate > calculatedRate;
        }

        public override string GetDescription()
        {
            return $"Max Percentage: {Percentage}%";
        }
    }
}
