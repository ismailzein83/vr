using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business.RouteRules.StructureRuleBehaviors
{
    public class RuleBehaviorByCustomer : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<int> keys)
        {
            RouteRule routeRule = rule as RouteRule;
            if (routeRule.RouteCriteria.CustomersGroupConfigId.HasValue)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                keys = carrierAccountManager.GetCustomerIds(routeRule.RouteCriteria.CustomersGroupConfigId.Value, routeRule.RouteCriteria.CustomerGroupSettings);
            }
            else
                keys = null;
        }

        protected override bool TryGetKeyFromTarget(object target, out int key)
        {
            RouteIdentifier routeIdentifier = target as RouteIdentifier;
            key = routeIdentifier.CustomerId;
            return true;
        }
    }
}
