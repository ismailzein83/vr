using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business.RouteRules.StructureRuleBehaviors
{
    public class RuleBehaviorByRoutingProduct : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<int> keys)
        {
            RouteRule routeRule = rule as RouteRule;
            if (routeRule.RouteCriteria.RoutingProductId.HasValue)
            {
                keys = new List<int> { routeRule.RouteCriteria.RoutingProductId.Value };
            }
            else
                keys = null;
        }

        protected override bool TryGetKeyFromTarget(object target, out int key)
        {
            RouteIdentifier routeIdentifier = target as RouteIdentifier;
            if (routeIdentifier.RoutingProductId.HasValue)
            {
                key = routeIdentifier.RoutingProductId.Value;
                return true;
            }
            else
            {
                key = 0;
                return false;
            }
        }
    }
}
