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
    public class RouteRuleCleanupManager
    {
        public void Cleanup(Dictionary<CleanupDataType, object> data)
        {
            object deletedSaleZoneIdsObj = null;
            if (data.TryGetValue(CleanupDataType.SaleZone, out deletedSaleZoneIdsObj))
            {
                if (deletedSaleZoneIdsObj != null)
                {
                    RouteRuleManager routeRuleManager = new RouteRuleManager();
                    IEnumerable<RouteRule> allRouteRules = routeRuleManager.GetAllRules().Values;

                    IEnumerable<long> deletedSaleZoneIds = deletedSaleZoneIdsObj as IEnumerable<long>;
                    ISaleZoneGroupCleanupContext context = new SaleZoneGroupCleanupContext() { DeletedSaleZoneIds = deletedSaleZoneIds };

                    foreach (RouteRule rule in allRouteRules)
                    {
                        if (rule.Criteria.SaleZoneGroupSettings != null)
                            rule.Criteria.SaleZoneGroupSettings.CleanDeletedZoneIds(context);
                    }

                }
            }
        }
    }
}
