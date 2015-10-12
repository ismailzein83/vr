using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business.RouteOptionRules.StructureRuleBehaviors
{
    public class RuleBehaviorBySupplierZone : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<long>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<long> keys)
        {
            keys = null;
            RouteOptionRule routeOptionRule = rule as RouteOptionRule;
            if (routeOptionRule.Criteria.SuppliersWithZonesGroupSettings != null)
            {
                SupplierZoneManager manager = new SupplierZoneManager();
                var suppliersWithZones = manager.GetSuppliersWithZoneIds(routeOptionRule.Criteria.SuppliersWithZonesGroupSettings);
                if (suppliersWithZones != null)
                    keys = suppliersWithZones.SelectMany(itm => itm.SupplierZoneIds != null ? itm.SupplierZoneIds : new List<long>());
            }
        }

        protected override bool TryGetKeyFromTarget(object target, out long key)
        {
            RouteOptionIdentifier routeOptionIdentifier = target as RouteOptionIdentifier;
            key = routeOptionIdentifier.SupplierZoneId;
            return true;
        }
    }
}
