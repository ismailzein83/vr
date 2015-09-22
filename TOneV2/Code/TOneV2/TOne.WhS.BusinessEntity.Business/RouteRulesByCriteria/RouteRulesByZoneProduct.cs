using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByProductZone : RouteRulesByTwoIds<int, long>
    {
        protected override bool IsRuleMatched(RouteRule rule, out IEnumerable<int> ids1, out IEnumerable<long> ids2)
        {
             if (rule.Criteria.RoutingProductId != null && rule.Criteria.HasZoneFilter())
             {
                 ids1 = new List<int> { rule.Criteria.RoutingProductId.Value };
                 ids2 = rule.Criteria.ZoneIds;
                 return true;
             }
             else
             {
                 ids1 = null;
                 ids2 = null;
                 return false;
             }
        }

        protected override bool AreIdsAvailable(int? customerId, int? productId, string code, long saleZoneId, out int id1, out long id2)
        {
            if (productId != null)
            {
                id1 = productId.Value;
                id2 = saleZoneId;
                return true;
            }
            else
            {
                id1 = 0;
                id2 = 0;
                return false;
            }
        }
    }
}
