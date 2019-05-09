using System;
using System.Linq;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation
{
    public class AverageTargetMatchCalculation : TargetMatchCalculationMethod
    {
        public override Guid ConfigId { get { return new Guid("2CB834AD-16BE-46C7-8B26-E65927C2388B"); } }

        public override void Evaluate(ITargetMatchCalculationMethodContext context)
        {
            if (context.RPRouteDetail == null || context.RPRouteDetail.RouteOptionsDetails == null || context.RPRouteDetail.RouteOptionsDetails.Count() == 0)
                return;

            var filteredRouteOptionDetails = context.RPRouteDetail.RouteOptionsDetails.FindAllRecords(itm => itm.ConvertedSupplierRate.HasValue);

            if (filteredRouteOptionDetails == null || filteredRouteOptionDetails.Count() == 0)
                return;

            decimal rate = filteredRouteOptionDetails.Average((x) => x.ConvertedSupplierRate.Value);
            context.TargetRates.Add(context.EvaluateRate(rate));
        }
    }
}