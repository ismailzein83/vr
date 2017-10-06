using System;
using System.Linq;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.PricingTemplateRate
{
    public class RouteOptionMarginRateCalculation : MarginRateCalculation
    {
        public override Guid ConfigId { get { return new Guid("3151D541-ADE3-4069-9203-D6C1873F0055"); } }

        public int RPRouteOptionNumber { get; set; }

        public override decimal? GetRate(IMarginRateCalculationContext context)
        {
            if (context.ZoneItem.RPRouteDetail != null && context.ZoneItem.RPRouteDetail.RouteOptionsDetails != null)
            {
                if (RPRouteOptionNumber <= context.ZoneItem.RPRouteDetail.RouteOptionsDetails.Count())
                {
                    RPRouteOptionDetail rpRouteOption = context.ZoneItem.RPRouteDetail.RouteOptionsDetails.ElementAt(RPRouteOptionNumber - 1);
                    return rpRouteOption.ConvertedSupplierRate;
                }
            }
            return null;
        }
    }
}
