using System;
using System.Linq;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.PricingTemplateRate
{
    public class CostMarginRateCalculation : MarginRateCalculation
    {
        public override Guid ConfigId { get { return new Guid("01090AC4-8995-485B-A5E1-7FD80271641C"); } }

        public Guid CostCalculationMethodConfigId { get; set; }

        public override decimal? GetRate(IMarginRateCalculationContext context)
        {
            if (context.ZoneItem.Costs != null)
            {
                int? costCalculationMethodIndex = context.GetCostCalculationMethodIndex(CostCalculationMethodConfigId);
                if (costCalculationMethodIndex.HasValue)
                    return context.ZoneItem.Costs.ElementAt(costCalculationMethodIndex.Value);
            }
            return null;
        }
    }
}