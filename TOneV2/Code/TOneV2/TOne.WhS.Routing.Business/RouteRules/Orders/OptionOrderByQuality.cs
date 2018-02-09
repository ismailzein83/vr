using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

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

            QualityConfigurationManager manager = new QualityConfigurationManager();
            List<IRouteOptionOrderTarget> suppliersNotFound = new List<IRouteOptionOrderTarget>();

            decimal? supplierTQI;
            decimal maxTQI = 0;
            foreach (IRouteOptionOrderTarget option in context.Options)
            {
                if (option.SupplierZoneId.HasValue)
                    supplierTQI = this.GetCustomerRouteQualityValue(option.SupplierZoneId.Value, manager, context.RoutingDatabase);
                else
                    supplierTQI = this.GetRoutingProductQualityValue(option.SaleZoneId.Value, option.SupplierId, manager, context.RoutingDatabase);

                if (supplierTQI.HasValue)
                {
                    option.OptionWeight = supplierTQI.Value;
                    maxTQI = Math.Max(maxTQI, supplierTQI.Value);
                }
                else
                {
                    suppliersNotFound.Add(option);
                }
            }

            if (suppliersNotFound.Count > 0)
            {
                foreach (IRouteOptionOrderTarget option in suppliersNotFound)
                    option.OptionWeight = maxTQI;
            }
        }

        private decimal? GetCustomerRouteQualityValue(long supplierZoneId, QualityConfigurationManager manager, RoutingDatabase routingDatabase)
        {
            Dictionary<long, List<CustomerRouteQualityConfigurationData>> customerRouteQualityConfigurationData = manager.GetCachedCustomerRouteQualityConfigurationData(routingDatabase);
            if (customerRouteQualityConfigurationData == null)
                return null;

            List<CustomerRouteQualityConfigurationData> customerRouteQualityConfigurationsData;
            if (!customerRouteQualityConfigurationData.TryGetValue(supplierZoneId, out customerRouteQualityConfigurationsData) || customerRouteQualityConfigurationsData.Count == 0)
                return null;

            decimal qualityValue = 0;

            foreach (var itm in customerRouteQualityConfigurationsData)
            {
                if (this.QualityConfigurationIds.Contains(itm.QualityConfigurationId))
                    qualityValue += itm.QualityData;
            }

            return qualityValue;
        }

        private decimal? GetRoutingProductQualityValue(long saleZoneId, int supplierId, QualityConfigurationManager manager, RoutingDatabase routingDatabase)
        {
            Dictionary<SaleZoneSupplier, List<RPQualityConfigurationData>> rpQualityConfigurationData = manager.GetCachedRPQualityConfigurationData(routingDatabase);
            if (rpQualityConfigurationData == null)
                return null;

            SaleZoneSupplier saleZoneSupplier = new SaleZoneSupplier() { SaleZoneId = saleZoneId, SupplierId = supplierId };

            List<RPQualityConfigurationData> rpQualityConfigurationsData;
            if (!rpQualityConfigurationData.TryGetValue(saleZoneSupplier, out rpQualityConfigurationsData) || rpQualityConfigurationsData.Count == 0)
                return null;

            decimal qualityValue = 0;

            foreach (var itm in rpQualityConfigurationsData)
            {
                if (this.QualityConfigurationIds.Contains(itm.QualityConfigurationId))
                    qualityValue += itm.QualityData;
            }

            return qualityValue;
        }
    }
}