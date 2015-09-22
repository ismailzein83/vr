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
        protected override bool IsRuleMatched(RouteRule rule, out IEnumerable<long> ids)
        {
            if (rule.Criteria.HasZoneFilter() && rule.Criteria.RoutingProductId == null && !rule.Criteria.HasCustomerFilter())
            {
                ids = rule.Criteria.ZoneIds;
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
