using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.CostCalculation
{
    public class AvgCostCalculation : CostCalculationMethod
    {
        public override Guid ConfigId { get { return new Guid("98B8E899-4ED7-4BCB-B6EF-8AE94E382E62"); } }
        public override void CalculateCost(ICostCalculationMethodContext context)
        {
            if (context.Route == null)
                throw new ArgumentNullException("context.Route");
            if (context.Route.RouteOptionsDetails != null && context.Route.RouteOptionsDetails.Count() > 0)
            {
                if (context.NumberOfOptions.HasValue && context.Route.RouteOptionsDetails.Count() >= context.NumberOfOptions.Value)
                    context.Cost = context.Route.RouteOptionsDetails.Take(context.NumberOfOptions.Value).Average(x => x.ConvertedSupplierRate);
                else context.Cost = context.Route.RouteOptionsDetails.Average(x => x.ConvertedSupplierRate);
            }
        }
    }
}
