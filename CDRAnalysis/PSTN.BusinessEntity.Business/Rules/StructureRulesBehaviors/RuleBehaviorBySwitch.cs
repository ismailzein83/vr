using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Business.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorBySwitch : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<int> keys)
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
    }
}
