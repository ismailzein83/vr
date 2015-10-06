using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities.Normalization.StructureRuleBehaviors
{
    public class RuleBehaviorByNumberLength : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<int> keys)
        {
            NormalizationRule normalizationRule = rule as NormalizationRule;
            if (normalizationRule.Criteria.PhoneNumberLength.HasValue)
                keys = new List<int> { normalizationRule.Criteria.PhoneNumberLength.Value };
            else
                keys = null;
        }

        protected override bool TryGetKeyFromTarget(object target, out int key)
        {
            CDRToNormalizeInfo cdr = target as CDRToNormalizeInfo;
            key = cdr.PhoneNumber.Length;
            return true;
        }
    }
}
