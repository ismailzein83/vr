using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities.Normalization.StructureRuleBehaviors
{
    public class RuleBehaviorByNumberType : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<NormalizationPhoneNumberType>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<NormalizationPhoneNumberType> keys)
        {
            NormalizationRule normalizationRule = rule as NormalizationRule;
            if (normalizationRule.Criteria.PhoneNumberType.HasValue)
                keys = new List<NormalizationPhoneNumberType> { normalizationRule.Criteria.PhoneNumberType.Value };
            else
                keys = null;
        }

        protected override bool TryGetKeyFromTarget(object target, out NormalizationPhoneNumberType key)
        {
            CDRToNormalizeInfo cdr = target as CDRToNormalizeInfo;
            if (cdr.PhoneNumberType.HasValue)
            {
                key = cdr.PhoneNumberType.Value;
                return true;
            }
            else
            {
                key = default(NormalizationPhoneNumberType);
                return false;
            }
        }
    }
}
