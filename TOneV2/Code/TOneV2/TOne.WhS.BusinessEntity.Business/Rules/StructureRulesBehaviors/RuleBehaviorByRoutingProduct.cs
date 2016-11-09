using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors
{
    public class RuleBehaviorByRoutingProduct : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.IVRRule rule, out IEnumerable<int> keys)
        {
            IRuleRoutingProductCriteria ruleRoutingProductCriteria = rule as IRuleRoutingProductCriteria;
            keys = ruleRoutingProductCriteria.RoutingProductIds;
        }

        protected override bool TryGetKeyFromTarget(object target, out int key)
        {
            IRuleRoutingProductTarget ruleRoutingProductTarget = target as IRuleRoutingProductTarget;
            if (ruleRoutingProductTarget.RoutingProductId.HasValue)
            {
                key = ruleRoutingProductTarget.RoutingProductId.Value;
                return true;
            }
            else
            {
                key = 0;
                return false;
            }
        }

        public override Vanrise.Rules.BaseRuleStructureBehavior CreateNewBehaviorObject()
        {
            return new RuleBehaviorByRoutingProduct();
        }
    }
}
