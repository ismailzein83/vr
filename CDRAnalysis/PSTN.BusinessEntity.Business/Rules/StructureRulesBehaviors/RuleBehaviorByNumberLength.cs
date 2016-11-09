using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Business.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByNumberLength : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.IVRRule rule, out IEnumerable<int> keys)
        {
            IRulePhoneNumberLengthCriteria rulePhoneNumberLengthCriteria = rule as IRulePhoneNumberLengthCriteria;
            keys = rulePhoneNumberLengthCriteria.PhoneNumberLengths;
        }

        protected override bool TryGetKeyFromTarget(object target, out int key)
        {
            IRulePhoneNumberTarget rulePhoneNumberTarget = target as IRulePhoneNumberTarget;
            if (rulePhoneNumberTarget.PhoneNumber != null)
            {
                key = rulePhoneNumberTarget.PhoneNumber.Length;
                return true;
            }
            else
            {
                key = 0;
                return false;
            }
        }

        public override Vanrise.Rules.BaseRuleStructureBehavior CreateNewBehaviorObject()
        {
            return new RuleBehaviorByNumberLength();
        }
    }
}
