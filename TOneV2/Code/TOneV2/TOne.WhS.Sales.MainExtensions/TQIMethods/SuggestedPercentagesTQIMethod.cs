using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class SuggestedPercentagesTQIMethod : TQIMethod
    {
        public override Guid ConfigId { get { return new Guid("C4D65558-F499-454E-906C-1B55E5DBC3CF"); } }
        public Dictionary<string, decimal> SuggestedPercentagesBySupplierName { get; set; }
        public override void CalculateRate(ITQIMethodContext context)
        {
            if (context.Route != null && context.Route.RouteOptionsDetails != null)
            {
                decimal rate = 0;
                foreach (RPRouteOptionDetail route in context.Route.RouteOptionsDetails)
                {
                    decimal currentSupplierPercentage;
                    if (this.SuggestedPercentagesBySupplierName.TryGetValue(route.SupplierName, out currentSupplierPercentage))
                        rate += route.ConvertedSupplierRate * currentSupplierPercentage;
                }

                context.Rate = rate / 100;
            }
        }
    }
}
