using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.CostCalculation
{
    public class PercentageCostCalculation : CostCalculationMethod
    {
        public override Guid ConfigId { get { return  new Guid("B1975F3C-528B-410C-ABDF-887DEC0D1B44"); } }
        public RouteOptionPercentageSettings PercentageSettings { get; set; }

        public override void CalculateCost(ICostCalculationMethodContext context)
        {
            if (context.Route == null)
                throw new ArgumentNullException("context.Route");
            if (context.Route.RouteOptionsDetails != null)
            {
                IEnumerable<RPRouteOptionDetail>routeOptionsDetails = new List<RPRouteOptionDetail>();
                if (context.NumberOfOptions.HasValue && context.Route.RouteOptionsDetails.Count() >= context.NumberOfOptions.Value)
                    routeOptionsDetails = context.Route.RouteOptionsDetails.Take(context.NumberOfOptions.Value);
                else routeOptionsDetails = context.Route.RouteOptionsDetails;
                List<RouteOptionPercentageTarget> percentageOptions = new List<RouteOptionPercentageTarget>();
                foreach(var percentageOption in routeOptionsDetails)
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
                    if(percentageOption.Percentage.HasValue)
                        cost += (option.ConvertedSupplierRate * percentageOption.Percentage.Value);
                    currentIndex++;
                }
                context.Cost = cost / 100;
            }
        }

        #region Private Classes

        private class RouteOptionPercentageTarget : IRouteOptionPercentageTarget
        {
            public int? Percentage
            {
                get;
                set;
            }
        }

        private class RouteOptionPercentageExecutionContext : IRouteOptionPercentageExecutionContext
        {
            public IEnumerable<IRouteOptionPercentageTarget> Options
            {
                get;
                set;
            }
        }

        #endregion
    }
}
