using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByCode<T> : RouteRulesByOneId<T, string> where T : IRouteCriteria
    {
        protected override bool IsRuleMatched(T rule, out IEnumerable<string> ids)
        {
            if (rule.RouteCriteria.HasCodeFilter() && !rule.RouteCriteria.HasCustomerFilter())
            {
                ids = rule.RouteCriteria.Codes.Select(code => code.Code);
                return true;
            }
            else
            {
                ids = null;
                return false;
            }
        }

        protected override bool IsIdAvailable(int? customerId, int? productId, string code, long saleZoneId, out string id)
        {
            id = code;
            return (id != null);
        }
    }
}
