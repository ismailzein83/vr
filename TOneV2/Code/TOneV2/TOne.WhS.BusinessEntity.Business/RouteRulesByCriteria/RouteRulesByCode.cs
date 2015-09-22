using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByCode : RouteRulesByOneId<string>
    {      
        protected override bool IsRuleMatched(RouteRule rule, out IEnumerable<string> ids)
        {
            if (rule.Criteria.HasCodeFilter() && !rule.Criteria.HasCustomerFilter())
            {
                ids = rule.Criteria.Codes.Select(code => code.Code);
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
