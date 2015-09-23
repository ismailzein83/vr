using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByCustomer<T> : RouteRulesByOneId<T, int> where T : IRouteCriteria
    {
        protected override bool IsRuleMatched(T rule, out IEnumerable<int> ids)
        {
            if (rule.RouteCriteria.HasCustomerFilter() && !rule.RouteCriteria.HasZoneFilter() && !rule.RouteCriteria.HasCodeFilter())
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                ids = carrierAccountManager.GetCustomerIds(rule.RouteCriteria.CustomersGroupConfigId.Value, rule.RouteCriteria.CustomerGroupSettings);
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
            if(customerId != null)
            {
                id = customerId.Value;
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
