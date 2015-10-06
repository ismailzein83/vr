using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities.Normalization.StructureRuleBehaviors
{
    public class RuleBehaviorByTrunk : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<int> keys)
        {
            NormalizationRule normalizationRule = rule as NormalizationRule;
            keys = normalizationRule.Criteria.TrunkIds;
        }

        protected override bool TryGetKeyFromTarget(object target, out int key)
        {
            CDRToNormalizeInfo cdr = target as CDRToNormalizeInfo;
            if (cdr.TrunkId.HasValue)
            {
                key = cdr.TrunkId.Value;
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
