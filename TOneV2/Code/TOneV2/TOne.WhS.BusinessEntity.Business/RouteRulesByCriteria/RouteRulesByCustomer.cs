﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteRulesByCustomer : RouteRulesByOneId<int>
    {
        protected override bool IsRuleMatched(RouteRule rule, out IEnumerable<int> ids)
        {
            if (rule.Criteria.HasCustomerFilter() && !rule.Criteria.HasZoneFilter() && !rule.Criteria.HasCodeFilter())
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                ids = carrierAccountManager.GetCustomerIds(rule.Criteria.CustomersGroupConfigId.Value, rule.Criteria.CustomerGroupSettings);
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
