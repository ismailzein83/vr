using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors
{
    public class RuleBehaviorBySaleZone : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<long>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.IVRRule rule, out IEnumerable<long> keys)
        {
            IRuleSaleZoneCriteria ruleSaleZoneCriteria = rule as IRuleSaleZoneCriteria;
            keys = ruleSaleZoneCriteria.SaleZoneIds;
        }

        protected override bool TryGetKeyFromTarget(object target, out long key)
        {
            IRuleSaleZoneTarget ruleSaleZoneTarget = target as IRuleSaleZoneTarget;
            if (ruleSaleZoneTarget.SaleZoneId.HasValue)
            {
                key = ruleSaleZoneTarget.SaleZoneId.Value;
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
            return new RuleBehaviorBySaleZone();
        }
    }
}
