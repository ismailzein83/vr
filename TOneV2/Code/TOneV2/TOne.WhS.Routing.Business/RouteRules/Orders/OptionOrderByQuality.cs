using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Orders
{
    public class OptionOrderByQuality : RouteOptionOrderSettings
    {
        public override void Execute(IRouteOptionOrderExecutionContext context)
        {
            context.OrderDitection = OrderDirection.Descending;
            TrafficStatsQualityMeasureManager manager = new TrafficStatsQualityMeasureManager();
            foreach (IRouteOptionOrderTarget option in context.Options)
            {
                if (option.SupplierZoneId.HasValue)
                    option.OptionWeight = manager.GetTrafficStatsQualityMeasure(option.SupplierZoneId.Value);
                else
                    option.OptionWeight = manager.GetTrafficStatsQualityMeasure(option.SaleZoneId.Value, option.SupplierId);
            }
        }
    }
}
