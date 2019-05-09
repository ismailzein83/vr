using System;
using TOne.WhS.Sales.Entities;
using System.Linq;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions.CostCalculation
{
    public class RoutePercentageCostCalculation : CostCalculationMethod
    {
        public override Guid ConfigId { get { return new Guid("8008678D-16D0-4026-8094-D8E7364E6F58"); } }

        public override void CalculateCost(ICostCalculationMethodContext context)
        {
            if (context.Route == null)
                throw new ArgumentNullException("context.Route");

            if (context.Route.RouteOptionsDetails == null || context.Route.RouteOptionsDetails.Count() == 0)
                return;

            var filteredRouteOptionDetails = context.Route.RouteOptionsDetails.FindAllRecords(itm => itm.SupplierRate.HasValue);

            if (filteredRouteOptionDetails == null || filteredRouteOptionDetails.Count() == 0)
                return;

            Decimal cost = 0;
            int totalPercentage = 0;

            foreach (var option in filteredRouteOptionDetails)
            {
                if (option.Percentage.HasValue)
                {
                    totalPercentage += option.Percentage.Value;
                    cost += (option.ConvertedSupplierRate.Value * option.Percentage.Value);
                }
            }

            if (totalPercentage > 0)
                context.Cost = cost / totalPercentage;
            else
                context.Cost = 0;
        }
    }
}