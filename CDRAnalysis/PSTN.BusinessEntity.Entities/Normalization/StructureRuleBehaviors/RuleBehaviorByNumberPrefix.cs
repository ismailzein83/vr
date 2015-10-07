using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities.Normalization.StructureRuleBehaviors
{
    public class RuleBehaviorByNumberPrefix : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByPrefix
    {
        protected override void GetPrefixesFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<string> prefixes)
        {
            NormalizationRule normalizationRule = rule as NormalizationRule;
            if (normalizationRule.Criteria.PhoneNumberPrefix != null)
                prefixes = new List<string> { normalizationRule.Criteria.PhoneNumberPrefix };
            else
                prefixes = null;
        }

        protected override bool TryGetValueToCompareFromTarget(object target, out string value)
        {
            CDRToNormalizeInfo cdr = target as CDRToNormalizeInfo;
            value = cdr.PhoneNumber;
            return true;
        }
    }
}
