using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Business.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorBySwitch : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.IVRRule rule, out IEnumerable<int> keys)
        {
            IRuleSwitchCriteria ruleSwitchCriteria = rule as IRuleSwitchCriteria;
            keys = ruleSwitchCriteria.SwitchIds;
        }

        protected override bool TryGetKeyFromTarget(object target, out int key)
        {
            IRuleSwitchTarget ruleSwitchTarget = target as IRuleSwitchTarget;
            if (ruleSwitchTarget.SwitchId.HasValue)
            {
                key = ruleSwitchTarget.SwitchId.Value;
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
            return new RuleBehaviorBySwitch();
        }
    }
}
