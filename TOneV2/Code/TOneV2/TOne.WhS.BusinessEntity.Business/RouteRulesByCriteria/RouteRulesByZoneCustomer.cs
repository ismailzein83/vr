using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByCustomerZone : RouteRulesByTwoIds<int, long>
    {
        protected override bool IsRuleMatched(RouteRule rule, out IEnumerable<int> ids1, out IEnumerable<long> ids2)
        {
            if (rule.Criteria.HasCustomerFilter() && rule.Criteria.HasZoneFilter())
            {
                ids1 = rule.Criteria.CustomerIds;
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
            if (customerId != null)
            {
                id1 = customerId.Value;
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
