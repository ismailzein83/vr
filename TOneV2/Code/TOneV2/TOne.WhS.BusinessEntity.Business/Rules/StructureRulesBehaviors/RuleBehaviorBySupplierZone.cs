using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors
{
    public class RuleBehaviorBySupplierZone : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<long>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.IVRRule rule, out IEnumerable<long> keys)
        {
            IRuleSupplierZoneCriteria ruleSupplierZoneCriteria = rule as IRuleSupplierZoneCriteria;
            keys = ruleSupplierZoneCriteria.SupplierZoneIds;
        }

        protected override bool TryGetKeyFromTarget(object target, out long key)
        {
            IRuleSupplierZoneTarget ruleSupplierZoneTarget = target as IRuleSupplierZoneTarget;
            if (ruleSupplierZoneTarget.SupplierZoneId.HasValue)
            {
                key = ruleSupplierZoneTarget.SupplierZoneId.Value;
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
            return new RuleBehaviorBySupplierZone();
        }
    }
}
