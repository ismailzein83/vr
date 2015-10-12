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
            IRuleCustomerCriteria ruleCustomerCriteria = rule as IRuleCustomerCriteria;
            if (ruleCustomerCriteria.CustomerGroupSettings != null)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                keys = carrierAccountManager.GetCustomerIds(ruleCustomerCriteria.CustomerGroupSettings.ConfigId, ruleCustomerCriteria.CustomerGroupSettings);
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
