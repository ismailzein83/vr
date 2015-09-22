using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByCustomerCode : RouteRulesByTwoIds<int, string>
    {
        protected override bool IsRuleMatched(RouteRule rule, out IEnumerable<int> ids1, out IEnumerable<string> ids2)
        {
            if (rule.Criteria.HasCustomerFilter() && rule.Criteria.HasCodeFilter())
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                ids1 = carrierAccountManager.GetCustomerIds(rule.Criteria.CustomersGroupConfigId.Value, rule.Criteria.CustomerGroupSettings);
                ids2 = rule.Criteria.Codes.Select(code => code.Code);
                return true;
            }
            else
            {
                ids1 = null;
                ids2 = null;
                return false;
            }
        }

        protected override bool AreIdsAvailable(int? customerId, int? productId, string code, long saleZoneId, out int id1, out string id2)
        {
            if (customerId != null && code != null)
            {
                id1 = customerId.Value;
                id2 = code;
                return true;
            }
            else
            {
                id1 = 0;
                id2 = null;
                return false;
            }
        }
    }
}
