using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities.Normalization.RuleSets
{
    public class RuleSetByTrunkLength : Vanrise.Rules.Entities.RuleSets.RuleSet2Dim<int, int>
    {
        public override void GetKeysFromRule(Vanrise.Rules.Entities.BaseRule r, out IEnumerable<int> keys1, out IEnumerable<int> keys2)
        {
            NormalizationRule normalizationRule = r as NormalizationRule;

            keys1 = normalizationRule.Criteria.TrunkIds;
            if (normalizationRule.Criteria.PhoneNumberLength.HasValue)
                keys2 = new List<int> { normalizationRule.Criteria.PhoneNumberLength.Value };
            else
                keys2 = null;
        }

        public override void GetKeysFromTarget(object target, out int key1, out int key2)
        {
            CDRToNormalizeInfo cdr = target as CDRToNormalizeInfo;
            key1 = cdr.TrunkId;
            key2 = cdr.PhoneNumber.Length;
        }
    }
}
