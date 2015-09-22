using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByProduct : RouteRulesByOneId<int>
    {
        protected override bool IsRuleMatched(IRouteCriteria rule, out IEnumerable<int> ids)
        {
            if (rule.RouteCriteria.RoutingProductId != null && !rule.RouteCriteria.HasZoneFilter())
            {
                ids = new List<int> { rule.RouteCriteria.RoutingProductId.Value };
                return true;
            }
            else
            {
                ids = null;
                return false;
            }
        }

        protected override bool IsIdAvailable(int? customerId, int? productId, string code, long saleZoneId, out int id)
        {
            if(productId != null)
            {
                id = productId.Value;
                return true;
            }
            else
            {
                id = 0;
                return false;
            }
        }
    }
}
