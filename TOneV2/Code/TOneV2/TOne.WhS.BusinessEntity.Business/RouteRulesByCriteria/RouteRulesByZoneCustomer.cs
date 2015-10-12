using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByCustomerZone<T> : RouteRulesByTwoIds<T, int, long> where T : IRouteCriteria
    {
        protected override bool IsRuleMatched(T rule, out IEnumerable<int> ids1, out IEnumerable<long> ids2)
        {
            if (rule.Criteria.HasCustomerFilter() && rule.Criteria.HasZoneFilter())
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                ids1 = carrierAccountManager.GetCustomerIds(rule.Criteria.CustomersGroupConfigId.Value, rule.Criteria.CustomerGroupSettings);
                SaleZoneManager saleZoneManager = new SaleZoneManager();
                ids2 = saleZoneManager.GetSaleZoneIds(rule.Criteria.SaleZoneGroupConfigId.Value, rule.Criteria.SaleZoneGroupSettings);
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
