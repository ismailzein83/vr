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
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("8008678D-16D0-4026-8094-D8E7364E6F58"); } }
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
