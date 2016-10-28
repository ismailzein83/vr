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
    public class StateBackupRoutingCleanupManager
    {
        public void CleanupRouteRules(IStateBackupCleanupContext context)
        {
            RouteRuleManager routeRuleManager = new RouteRuleManager();
            IEnumerable<RouteRule> allRouteRules = routeRuleManager.GetAllRules().Values;

            ISaleZoneGroupCleanupContext saleZoneGroupContext = new SaleZoneGroupCleanupContext() { DeletedSaleZoneIds = context.SaleZoneIds };
            foreach (RouteRule rule in allRouteRules)
            {
                if (rule.Criteria.SaleZoneGroupSettings != null)
                {
                    rule.Criteria.SaleZoneGroupSettings.CleanDeletedZoneIds(saleZoneGroupContext);
                    //TODO: to implement updating rule process by MJA
                }
                    
            }
        }

        public void CleanupRouteOptionRules(IStateBackupCleanupContext context)
        {
            RouteOptionRuleManager routeRuleManager = new RouteOptionRuleManager();
            IEnumerable<RouteOptionRule> allRules = routeRuleManager.GetAllRules().Values;

            ISaleZoneGroupCleanupContext saleZoneCleanupContext = new SaleZoneGroupCleanupContext() { DeletedSaleZoneIds = context.SaleZoneIds };
            ISupplierZoneGroupCleanupContext supplierZoneCleanupContext = new SupplierZoneGroupCleanupContext() { DeletedSupplierZoneIds = context.SupplierZoneIds };

            foreach (RouteOptionRule rule in allRules)
            {
                if (rule.Criteria != null)
                {
                    //TODO: to implement updating rule process by MJA
                    if (context.SaleZoneIds != null && rule.Criteria.SaleZoneGroupSettings != null)
                        rule.Criteria.SaleZoneGroupSettings.CleanDeletedZoneIds(saleZoneCleanupContext);

                    if (context.SupplierZoneIds != null && rule.Criteria.SuppliersWithZonesGroupSettings != null)
                        rule.Criteria.SuppliersWithZonesGroupSettings.CleanDeletedZoneIds(supplierZoneCleanupContext);
                }
            }
        }
    }
}
