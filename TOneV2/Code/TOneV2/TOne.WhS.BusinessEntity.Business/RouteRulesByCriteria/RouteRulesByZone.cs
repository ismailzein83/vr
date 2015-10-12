using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByZone<T> : RouteRulesByOneId<T, long> where T : IRouteCriteria
    {
        protected override bool IsRuleMatched(T rule, out IEnumerable<long> ids)
        {
            if (rule.Criteria.HasZoneFilter() && rule.Criteria.RoutingProductId == null && !rule.Criteria.HasCustomerFilter())
            {
                SaleZoneManager saleZoneManager = new SaleZoneManager();
                ids = saleZoneManager.GetSaleZoneIds(rule.Criteria.SaleZoneGroupConfigId.Value, rule.Criteria.SaleZoneGroupSettings);
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
