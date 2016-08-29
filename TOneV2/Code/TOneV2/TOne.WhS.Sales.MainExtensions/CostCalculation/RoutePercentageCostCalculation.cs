using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.CostCalculation
{
    public class RoutePercentageCostCalculation : CostCalculationMethod
    {
        public override void CalculateCost(ICostCalculationMethodContext context)
        {
            if (context.Route == null)
                throw new ArgumentNullException("context.Route");
            if (context.Route.RouteOptionsDetails != null)
            {
                Decimal cost = 0;
                foreach (var option in context.Route.RouteOptionsDetails)
                {
                    if (option.Entity.Percentage.HasValue)
                        cost += (option.ConvertedSupplierRate * option.Entity.Percentage.Value);
                }
                context.Cost = cost / 100;
            }
        }
    }
}
