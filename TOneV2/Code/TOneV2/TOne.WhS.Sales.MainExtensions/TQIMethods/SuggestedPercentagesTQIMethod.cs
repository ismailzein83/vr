using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions
{
    public class SuggestedPercentagesTQIMethod : TQIMethod
    {
        public override Guid ConfigId { get { return new Guid("C4D65558-F499-454E-906C-1B55E5DBC3CF"); } }

        public List<TQISupplierPercentage> SuppliersPercentages { get; set; }

        public override void CalculateRate(ITQIMethodContext context)
        {
            if (context.Route == null || context.Route.RouteOptionsDetails == null)
                return;

            var filteredRouteOptionDetails = context.Route.RouteOptionsDetails.FindAllRecords(itm => itm.ConvertedSupplierRate.HasValue);

            if (filteredRouteOptionDetails == null || filteredRouteOptionDetails.Count() == 0)
                return;

            decimal rate = 0;
            foreach (RPRouteOptionDetail route in filteredRouteOptionDetails)
            {
                TQISupplierPercentage supplierPercentage = this.SuppliersPercentages.FindRecord(itm => itm.SupplierName.Equals(route.SupplierName, StringComparison.InvariantCultureIgnoreCase));
                if (supplierPercentage != null)
                    rate += route.ConvertedSupplierRate.Value * supplierPercentage.MarginPercentage;
            }

            context.Rate = rate / 100;
        }
    }
}