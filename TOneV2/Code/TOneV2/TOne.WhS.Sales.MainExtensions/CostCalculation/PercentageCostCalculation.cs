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
        public RouteOptionPercentageSettings PercentageSettings { get; set; }

        public override void CalculateCost(ICostCalculationMethodContext context)
        {
            if (context.Route == null)
                throw new ArgumentNullException("context.Route");
            if (context.Route.RouteOptionsDetails != null)
            {
                List<RouteOptionPercentageTarget> percentageOptions = new List<RouteOptionPercentageTarget>();
                foreach(var percentageOption in context.Route.RouteOptionsDetails)
                {
                    percentageOptions.Add(new RouteOptionPercentageTarget());
                }
                var percentageExecutionContext = new RouteOptionPercentageExecutionContext { Options = percentageOptions };
                this.PercentageSettings.Execute(percentageExecutionContext);
                Decimal cost = 0;
                int currentIndex = 0;
                foreach (var option in context.Route.RouteOptionsDetails)
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
            public decimal? Percentage
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
