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
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("98B8E899-4ED7-4BCB-B6EF-8AE94E382E62"); } }
        public override void CalculateCost(ICostCalculationMethodContext context)
        {
            if (context.Route == null)
                throw new ArgumentNullException("context.Route");
            if (context.Route.RouteOptionsDetails != null && context.Route.RouteOptionsDetails.Count() > 0)
                context.Cost = context.Route.RouteOptionsDetails.Average(x => x.ConvertedSupplierRate);
        }
    }
}
