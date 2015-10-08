using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business.RouteRules.StructureRuleBehaviors
{
    public class RuleBehaviorByZone : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<long>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<long> keys)
        {
            RouteRule routeRule = rule as RouteRule;
            if (routeRule.RouteCriteria.SaleZoneGroupConfigId.HasValue)
            {
                SaleZoneManager saleZoneManager = new SaleZoneManager();
                keys = saleZoneManager.GetSaleZoneIds(routeRule.RouteCriteria.SaleZoneGroupConfigId.Value, routeRule.RouteCriteria.SaleZoneGroupSettings);
            }
            else
                keys = null;
        }

        protected override bool TryGetKeyFromTarget(object target, out long key)
        {
            RouteIdentifier routeIdentifier = target as RouteIdentifier;
            key = routeIdentifier.SaleZoneId;
            return true;
        }
    }
}
