using System;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Orders
{
    public class OptionOrderByQuality : RouteOptionOrderSettings
    {
        public static Guid s_ConfigId { get { return new Guid("c107e207-6597-4e45-b22f-d4f0bb7dd211"); } }
        public override Guid ConfigId { get { return s_ConfigId; } }

        public Guid? QualityConfigurationId { get; set; }

        public override void Execute(IRouteOptionOrderExecutionContext context)
        {
            context.OrderDirection = OrderDirection.Descending;

            QualityConfigurationManager manager = new QualityConfigurationManager();

            decimal? supplierTQI;

            var defaultRouteRuleQualityConfiguration = new ConfigManager().GetDefaultRouteRuleQualityConfiguration();

            foreach (IRouteOptionOrderTarget option in context.Options)
            {
                if (option.SupplierZoneId.HasValue)
                    supplierTQI = manager.GetCustomerRouteQualityValue(option.SupplierZoneId.Value, context.RoutingDatabase, defaultRouteRuleQualityConfiguration, this.QualityConfigurationId);
                else
                    supplierTQI = manager.GetRoutingProductQualityValue(option.SaleZoneId, option.SupplierId, context.RoutingDatabase, defaultRouteRuleQualityConfiguration, this.QualityConfigurationId);

                if (supplierTQI.HasValue)
                    option.OptionWeight = supplierTQI.Value;
                else
                    option.OptionWeight = 0;
            }
        }
    }
}