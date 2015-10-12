using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business.RouteOptionRules.StructureRuleBehaviors
{
    public class RuleBehaviorBySupplier : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<int> keys)
        {
            keys = null;
            RouteOptionRule routeOptionRule = rule as RouteOptionRule;
            if(routeOptionRule.Criteria.SuppliersWithZonesGroupSettings != null)
            {
                SupplierZoneManager manager = new SupplierZoneManager();
                var suppliersWithZones = manager.GetSuppliersWithZoneIds(routeOptionRule.Criteria.SuppliersWithZonesGroupSettings);
                if (suppliersWithZones != null)
                    keys = suppliersWithZones.Select(itm => itm.SupplierId);
            }
        }

        protected override bool TryGetKeyFromTarget(object target, out int key)
        {
            RouteOptionIdentifier routeOptionIdentifier = target as RouteOptionIdentifier;
            key = routeOptionIdentifier.SupplierId;
            return true;
        }
    }
}
