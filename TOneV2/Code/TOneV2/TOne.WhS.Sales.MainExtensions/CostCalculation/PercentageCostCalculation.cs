using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions.CostCalculation
{
    public class PercentageCostCalculation : CostCalculationMethod
    {
        public override Guid ConfigId { get { return new Guid("B1975F3C-528B-410C-ABDF-887DEC0D1B44"); } }

        public RouteOptionPercentageSettings PercentageSettings { get; set; }

        public override void CalculateCost(ICostCalculationMethodContext context)
        {
            if (context.Route == null)
                throw new ArgumentNullException("context.Route");

            if (context.Route.RouteOptionsDetails == null || context.Route.RouteOptionsDetails.Count() == 0)
                return;

            var filteredRouteOptionDetails = context.Route.RouteOptionsDetails.FindAllRecords(itm => itm.SupplierRate.HasValue);

            if (filteredRouteOptionDetails == null || filteredRouteOptionDetails.Count() == 0)
                return;

            IEnumerable<RPRouteOptionDetail> routeOptionsDetails = new List<RPRouteOptionDetail>();

            if (context.NumberOfOptions.HasValue && filteredRouteOptionDetails.Count() >= context.NumberOfOptions.Value)
                routeOptionsDetails = filteredRouteOptionDetails.Take(context.NumberOfOptions.Value);
            else
                routeOptionsDetails = filteredRouteOptionDetails;

            List<RouteOptionPercentageTarget> percentageOptions = new List<RouteOptionPercentageTarget>();
            foreach (var percentageOption in routeOptionsDetails)
            {
                percentageOptions.Add(new RouteOptionPercentageTarget());
            }

            var percentageExecutionContext = new RouteOptionPercentageExecutionContext { Options = percentageOptions };
            this.PercentageSettings.Execute(percentageExecutionContext);

            Decimal cost = 0;
            int currentIndex = 0;

            foreach (var option in routeOptionsDetails)
            {
                var percentageOption = percentageOptions[currentIndex];

                if (percentageOption.Percentage.HasValue)
                    cost += (option.ConvertedSupplierRate.Value * percentageOption.Percentage.Value);

                currentIndex++;
            }

            context.Cost = cost / 100;
        }

        #region Private Classes

        private class RouteOptionPercentageTarget : IRouteOptionPercentageTarget
        {
            public int? Percentage { get; set; }
        }

        private class RouteOptionPercentageExecutionContext : IRouteOptionPercentageExecutionContext
        {
            public IEnumerable<IRouteOptionPercentageTarget> Options { get; set; }
        }

        #endregion
    }
}