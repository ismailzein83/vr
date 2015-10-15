using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Business.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByTrunk : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<int> keys)
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
    }
}
