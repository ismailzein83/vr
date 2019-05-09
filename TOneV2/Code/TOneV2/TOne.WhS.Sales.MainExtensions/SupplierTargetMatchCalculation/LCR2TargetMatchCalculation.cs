using System;
using System.Linq;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation
{
    public class LCR2TargetMatchCalculation : TargetMatchCalculationMethod
    {
        public override Guid ConfigId { get { return new Guid("E393CCB2-E929-4A50-9AE4-AB59565DD69D"); } }

        public override void Evaluate(ITargetMatchCalculationMethodContext context)
        {
            if (context.RPRouteDetail == null || context.RPRouteDetail.RouteOptionsDetails == null || context.RPRouteDetail.RouteOptionsDetails.Count() == 0)
                return;

            var filteredRouteOptionDetails = context.RPRouteDetail.RouteOptionsDetails.FindAllRecords(itm => itm.ConvertedSupplierRate.HasValue);

            if (filteredRouteOptionDetails == null || filteredRouteOptionDetails.Count() == 0)
                return;

            for (int i = 0; i < 2; i++)
            {
                RPRouteOptionDetail lcr = filteredRouteOptionDetails.ElementAtOrDefault(i);
                if (lcr != null)
                    context.TargetRates.Add(context.EvaluateRate(lcr.ConvertedSupplierRate.Value));
            }

        }
    }
}
