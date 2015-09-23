using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByZone : RouteRulesByOneId<long>
    {
        protected override bool IsRuleMatched(IRouteCriteria rule, out IEnumerable<long> ids)
        {
            if (rule.RouteCriteria.HasZoneFilter() && rule.RouteCriteria.RoutingProductId == null && !rule.RouteCriteria.HasCustomerFilter())
            {
                SaleZoneManager saleZoneManager = new SaleZoneManager();
                ids = saleZoneManager.GetSaleZoneIds(rule.RouteCriteria.SaleZoneGroupConfigId.Value, rule.RouteCriteria.SaleZoneGroupSettings);
                return true;
            }
            else
            {
                ids = null;
                return false;
            }
        }

        protected override bool IsIdAvailable(int? customerId, int? productId, string code, long saleZoneId, out long id)
        {
            id = saleZoneId;
            return true;
        }
    }
}
