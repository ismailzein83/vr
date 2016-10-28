using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.Extensions
{
    public class RouteOptionRuleCleanupManager
    {
        public void Cleanup(Dictionary<CleanupDataType, object> data)
        {
            object deletedSaleZoneIdsObj = null;
            data.TryGetValue(CleanupDataType.SaleZone, out deletedSaleZoneIdsObj);

            IEnumerable<long> deletedSaleZoneIds = null;
            if (deletedSaleZoneIdsObj != null)
                deletedSaleZoneIds = deletedSaleZoneIdsObj as IEnumerable<long>;

            object deletedSupplierZoneIdsObj = null;
            data.TryGetValue(CleanupDataType.SaleZone, out deletedSupplierZoneIdsObj);

            IEnumerable<long> deletedSupplierZoneIds = null;
            if (deletedSupplierZoneIdsObj != null)
                deletedSupplierZoneIds = deletedSupplierZoneIdsObj as IEnumerable<long>;


            if (deletedSaleZoneIds != null || deletedSupplierZoneIds != null)
            {
                RouteOptionRuleManager routeRuleManager = new RouteOptionRuleManager();
                IEnumerable<RouteOptionRule> allRules = routeRuleManager.GetAllRules().Values;

                ISaleZoneGroupCleanupContext saleZoneCleanupContext = new SaleZoneGroupCleanupContext() { DeletedSaleZoneIds = deletedSaleZoneIds };
                ISupplierZoneGroupCleanupContext supplierZoneCleanupContext = new SupplierZoneGroupCleanupContext() { DeletedSupplierZoneIds = deletedSupplierZoneIds };

                foreach (RouteOptionRule rule in allRules)
                {
                    if (rule.Criteria != null)
                    {
                        if (deletedSaleZoneIds != null && rule.Criteria.SaleZoneGroupSettings != null)
                            rule.Criteria.SaleZoneGroupSettings.CleanDeletedZoneIds(saleZoneCleanupContext);

                        if (deletedSupplierZoneIdsObj != null && rule.Criteria.SuppliersWithZonesGroupSettings != null)
                            rule.Criteria.SuppliersWithZonesGroupSettings.CleanDeletedZoneIds(supplierZoneCleanupContext);
                    }
                }
            }
        }
    }
}
