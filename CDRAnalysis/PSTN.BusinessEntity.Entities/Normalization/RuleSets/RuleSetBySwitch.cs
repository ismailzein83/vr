using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities.Normalization.RuleSets
{
    public class RuleSetBySwitch : Vanrise.Rules.Entities.RuleSets.RuleSet1Dim<int>
    {
        public override void GetKeysFromRule(Vanrise.Rules.Entities.BaseRule r, out IEnumerable<int> keys)
        {
            NormalizationRule normalizationRule = r as NormalizationRule;

            keys = normalizationRule.Criteria.SwitchIds;
        }

        public override void GetKeysFromTarget(object target, out int key)
        {
            CDRToNormalizeInfo cdr = target as CDRToNormalizeInfo;
            key = cdr.SwitchId;
        }
    }
}
