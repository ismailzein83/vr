using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities.CostCalculation.Extensions
{
    public class AvgCostCalculation : CostCalculationMethod
    {

        public override void CalculateCost(ICostCalculationMethodContext context)
        {
            if (context.Route == null)
                throw new ArgumentNullException("context.Route");
            if (context.Route.RouteOptionsDetails != null)
                context.Cost = context.Route.RouteOptionsDetails.Average(itm => itm.Entity.SupplierRate);
        }
    }
}
