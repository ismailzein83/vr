using System;
using System.Linq;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation
{
    public class WeightedAverageTargetMatchCalculation : TargetMatchCalculationMethod
    {
        public override Guid ConfigId
        {
            get { return new Guid("F8C397CD-1930-4E02-811F-DE2CB203EB85"); }
        }

        public override void Evaluate(ITargetMatchCalculationMethodContext context)
        {
            if (context.RPRouteDetail == null || context.RPRouteDetail.RouteOptionsDetails == null || context.RPRouteDetail.RouteOptionsDetails.Count() == 0)
                return;

            var filteredRouteOptionDetails = context.RPRouteDetail.RouteOptionsDetails.FindAllRecords(itm => itm.ConvertedSupplierRate.HasValue);

            if (filteredRouteOptionDetails == null || filteredRouteOptionDetails.Count() == 0)
                return;

            decimal sumOfDuration = 0;
            decimal sumOfRatesMultipliedByDuration = 0;

            foreach (RPRouteOptionDetail option in filteredRouteOptionDetails)
            {
                SupplierTargetMatchAnalyticItem supplierTargetMatchAnalyticItem;
                if (context.SupplierAnalyticDetail.TryGetValue(option.SupplierId, out supplierTargetMatchAnalyticItem))
                {
                    sumOfDuration += supplierTargetMatchAnalyticItem.Duration;
                    sumOfRatesMultipliedByDuration += supplierTargetMatchAnalyticItem.Duration * option.ConvertedSupplierRate.Value;
                }
            }

            if (sumOfDuration > 0)
            {
                context.TargetRates.Add(context.EvaluateRate(sumOfRatesMultipliedByDuration / sumOfDuration));
            }
            else
            {
                decimal rate = filteredRouteOptionDetails.Average((x) => x.ConvertedSupplierRate.Value);
                context.TargetRates.Add(context.EvaluateRate(rate));
            }
        }
    }
}