using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities.Normalization.RuleSets
{
    public class RuleSetBySwitchTrunkLength : Vanrise.Rules.Entities.RuleSets.RuleSet3Dim<int, int, int>
    {

        public override void GetKeysFromRule(Vanrise.Rules.Entities.BaseRule r, out IEnumerable<int> keys1, out IEnumerable<int> keys2, out IEnumerable<int> keys3)
        {
            NormalizationRule normalizationRule = r as NormalizationRule;

            keys1 = normalizationRule.Criteria.SwitchIds;
            keys2 = normalizationRule.Criteria.TrunkIds;
            if (normalizationRule.Criteria.PhoneNumberLength.HasValue)
                keys3 = new List<int> { normalizationRule.Criteria.PhoneNumberLength.Value };
            else
                keys3 = null;
        }

        public override void GetKeysFromTarget(object target, out int key1, out int key2, out int key3)
        {
            CDRToNormalizeInfo cdr = target as CDRToNormalizeInfo;
            key1 = cdr.SwitchId;
            key2 = cdr.TrunkId;
            key3 = cdr.PhoneNumber.Length;
        }
    }
}
