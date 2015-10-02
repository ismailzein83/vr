using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities.Normalization.RuleSets
{
    public class RuleSetByLength : Vanrise.Rules.Entities.RuleSets.RuleSet1Dim<int>
    {
        public override void GetKeysFromRule(Vanrise.Rules.Entities.BaseRule r, out IEnumerable<int> keys)
        {
            NormalizationRule normalizationRule = r as NormalizationRule;
      
            if (normalizationRule.Criteria.PhoneNumberLength.HasValue)
                keys = new List<int> { normalizationRule.Criteria.PhoneNumberLength.Value };
            else
                keys = null;
        }

        public override void GetKeysFromTarget(object target, out int key)
        {
            CDRToNormalizeInfo cdr = target as CDRToNormalizeInfo;
            key = cdr.PhoneNumber.Length;
        }
    }
}
