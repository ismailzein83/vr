using System;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Filters
{
    public enum RateOption { MinimumProfit = 0, MaximumLoss = 1 }
    public enum RateOptionType { Fixed = 0, Percentage = 1 }
    public class RateOptionFilter : RouteOptionFilterSettings
    {
        public RateOption RateOption { get; set; }
        public RateOptionType RateOptionType { get; set; }
        public decimal RateOptionValue { get; set; }
        public override void Execute(IRouteOptionFilterExecutionContext context)
        {
            if (!context.SaleRate.HasValue)
                return;

            decimal profitValue = 0;
            switch (RateOptionType)
            {
                case Filters.RateOptionType.Fixed: profitValue = context.SaleRate.Value - context.Option.SupplierRate; break;
                case Filters.RateOptionType.Percentage:
                    if (context.SaleRate.Value == 0)
                    {
                        context.FilterOption = true;
                        return;
                    }
                    profitValue = (context.SaleRate.Value - context.Option.SupplierRate) * 100 / context.SaleRate.Value; break;
            }

            decimal valueLimit = 0;
            switch (RateOption)
            {
                case Filters.RateOption.MaximumLoss: valueLimit = (-1) * RateOptionValue; break;
                case Filters.RateOption.MinimumProfit: valueLimit = RateOptionValue; break;
            }

            if (profitValue < valueLimit)
            {
                context.FilterOption = true;
            }
        }
    }
}