using System;
using System.Linq;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions.CostCalculation
{
    public class AvgCostCalculation : CostCalculationMethod
    {
        public override Guid ConfigId { get { return new Guid("98B8E899-4ED7-4BCB-B6EF-8AE94E382E62"); } }

        public override void CalculateCost(ICostCalculationMethodContext context)
        {
            if (context.Route == null)
                throw new ArgumentNullException("context.Route");

            if (context.Route.RouteOptionsDetails == null || context.Route.RouteOptionsDetails.Count() == 0)
                return;

            var filteredRouteOptionDetails = context.Route.RouteOptionsDetails.FindAllRecords(itm => itm.SupplierRate.HasValue);

            if (filteredRouteOptionDetails == null || filteredRouteOptionDetails.Count() == 0)
                return;

            if (context.NumberOfOptions.HasValue && filteredRouteOptionDetails.Count() >= context.NumberOfOptions.Value)
                context.Cost = filteredRouteOptionDetails.Take(context.NumberOfOptions.Value).Average(x => x.ConvertedSupplierRate.Value);
            else
                context.Cost = filteredRouteOptionDetails.Average(x => x.ConvertedSupplierRate.Value);
        }
    }
}