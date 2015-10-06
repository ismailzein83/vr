using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities.Normalization.StructureRuleBehaviors
{
    public class RuleBehaviorBySwitch : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<int> keys)
        {
            NormalizationRule normalizationRule = rule as NormalizationRule;
            keys = normalizationRule.Criteria.SwitchIds;
        }

        protected override bool TryGetKeyFromTarget(object target, out int key)
        {
            CDRToNormalizeInfo cdr = target as CDRToNormalizeInfo;
            if(cdr.SwitchId.HasValue)
            {
                key = cdr.SwitchId.Value;
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
