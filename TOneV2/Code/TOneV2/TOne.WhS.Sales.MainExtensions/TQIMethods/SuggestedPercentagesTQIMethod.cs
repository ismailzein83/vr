using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Analytic.Business;

namespace TOne.WhS.Sales.MainExtensions
{
    public class SuggestedPercentagesTQIMethod : TQIMethod
    {
        public override Guid ConfigId { get { return new Guid("C4D65558-F499-454E-906C-1B55E5DBC3CF"); } }
        public List<TQISupplierPercentage> SuppliersPercentages { get; set; }
        public override void CalculateRate(ITQIMethodContext context)
        {
            if (context.Route != null && context.Route.RouteOptionsDetails != null)
            {
                decimal rate = 0;
                foreach (RPRouteOptionDetail route in context.Route.RouteOptionsDetails)
                {
                    TQISupplierPercentage supplierPercentage = this.SuppliersPercentages.FindRecord(itm => itm.SupplierName.Equals(route.SupplierName, StringComparison.InvariantCultureIgnoreCase));
                    if(supplierPercentage != null)
                        rate += route.ConvertedSupplierRate * supplierPercentage.MarginPercentage;
                }

                context.Rate = rate / 100;
            }
        }
    }
}
