using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Business.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByNumberPrefix : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByPrefix
    {
        protected override void GetPrefixesFromRule(Vanrise.Rules.IVRRule rule, out IEnumerable<string> prefixes)
        {
            IRulePhoneNumberPrefixCriteria rulePhoneNumberPrefixCriteria = rule as IRulePhoneNumberPrefixCriteria;
            prefixes = rulePhoneNumberPrefixCriteria.PhoneNumberPrefixes;
        }

        protected override bool TryGetValueToCompareFromTarget(object target, out string value)
        {
            IRulePhoneNumberTarget rulePhoneNumberTarget = target as IRulePhoneNumberTarget;
            if (rulePhoneNumberTarget.PhoneNumber != null)
            {
                value = rulePhoneNumberTarget.PhoneNumber;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public override Vanrise.Rules.BaseRuleStructureBehavior CreateNewBehaviorObject()
        {
            return new RuleBehaviorByNumberPrefix();
        }
    }
}
