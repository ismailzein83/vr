using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Business.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByTrunk : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.IVRRule rule, out IEnumerable<int> keys)
        {
            IRuleTrunkCriteria tuleTrunkCriteria = rule as IRuleTrunkCriteria;
            keys = tuleTrunkCriteria.TrunkIds;
        }

        protected override bool TryGetKeyFromTarget(object target, out int key)
        {
            IRuleTrunkTarget ruleTrunkTarget = target as IRuleTrunkTarget;
            if (ruleTrunkTarget.TrunkId.HasValue)
            {
                key = ruleTrunkTarget.TrunkId.Value;
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
            return new RuleBehaviorByTrunk();
        }
    }
}
