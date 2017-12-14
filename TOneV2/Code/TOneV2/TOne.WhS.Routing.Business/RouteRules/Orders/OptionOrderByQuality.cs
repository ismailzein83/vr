using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Orders
{
    public class OptionOrderByQuality : RouteOptionOrderSettings
    {
        public static Guid s_ConfigId { get { return new Guid("c107e207-6597-4e45-b22f-d4f0bb7dd211"); } }
        public override Guid ConfigId { get { return s_ConfigId; } }

        public List<Guid> QualityConfigurationIds { get; set; }

        public override void Execute(IRouteOptionOrderExecutionContext context)
        {
            context.OrderDitection = OrderDirection.Descending;
            TrafficStatsQualityMeasureManager manager = new TrafficStatsQualityMeasureManager();
            List<IRouteOptionOrderTarget> suppliersNotFound = new List<IRouteOptionOrderTarget>();

            decimal? supplierTQI;
            decimal maxTQI = 0;
            foreach (IRouteOptionOrderTarget option in context.Options)
            {
                if (option.SupplierZoneId.HasValue)
                    supplierTQI = manager.GetTrafficStatsQualityMeasure(option.SupplierZoneId.Value);
                else
                    supplierTQI = manager.GetTrafficStatsQualityMeasure(option.SaleZoneId.Value, option.SupplierId);

                if (supplierTQI.HasValue)
                {
                    option.OptionWeight = supplierTQI.Value;
                    maxTQI = Math.Max(maxTQI, supplierTQI.Value);
                }
                else
                    suppliersNotFound.Add(option);
            }

            if (suppliersNotFound.Count > 0)
            {
                foreach (IRouteOptionOrderTarget option in suppliersNotFound)
                {
                    option.OptionWeight = maxTQI;
                }
            }
        }
    }
}
