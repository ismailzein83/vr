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
        protected override bool IsRuleMatched(IRouteCriteria rule, out IEnumerable<int> ids1, out IEnumerable<long> ids2)
        {
            if (rule.RouteCriteria.HasCustomerFilter() && rule.RouteCriteria.HasZoneFilter())
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                ids1 = carrierAccountManager.GetCustomerIds(rule.RouteCriteria.CustomersGroupConfigId.Value, rule.RouteCriteria.CustomerGroupSettings);
                ids2 = rule.RouteCriteria.ZoneIds;
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
