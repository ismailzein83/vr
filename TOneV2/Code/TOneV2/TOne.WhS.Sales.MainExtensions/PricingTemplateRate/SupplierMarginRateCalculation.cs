using System;
using Vanrise.Common;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.PricingTemplateRate
{
    public class SupplierMarginRateCalculation : MarginRateCalculation
    {
        public override Guid ConfigId { get { return new Guid("188AFED5-4161-4BF4-9E76-89BC25640881"); } }

        public int SupplierId { get; set; }

        public override decimal? GetRate(IMarginRateCalculationContext context)
        {
            if (context.ZoneItem.RPRouteDetail != null && context.ZoneItem.RPRouteDetail.RouteOptionsDetails != null)
            {
                RPRouteOptionDetail rpRouteOption = context.ZoneItem.RPRouteDetail.RouteOptionsDetails.FindRecord(x => x.SupplierId == SupplierId);
                if (rpRouteOption != null)
                    return rpRouteOption.ConvertedSupplierRate;
            }
            return null;
        }
    }
}