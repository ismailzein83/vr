using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.CostCalculation
{
    public class AvgCostCalculation : CostCalculationMethod
    {
        public override void CalculateCost(ICostCalculationMethodContext context)
        {
            if (context.Route == null)
                throw new ArgumentNullException("context.Route");
            if (context.Route.RouteOptionsDetails != null && context.Route.RouteOptionsDetails.Count() > 0)
                context.Cost = context.Route.RouteOptionsDetails.Average(x => x.ConvertedSupplierRate);
        }
    }
}
