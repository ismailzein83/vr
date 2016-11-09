using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Business.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByNumberType : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<NormalizationPhoneNumberType>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.IVRRule rule, out IEnumerable<NormalizationPhoneNumberType> keys)
        {
            IRulePhoneNumberTypeCriteria rulePhoneNumberTypeCriteria = rule as IRulePhoneNumberTypeCriteria;
            keys = rulePhoneNumberTypeCriteria.PhoneNumberTypes;
        }

        protected override bool TryGetKeyFromTarget(object target, out NormalizationPhoneNumberType key)
        {
            IRulePhoneNumberTypeTarget rulePhoneNumberTypeTarget = target as IRulePhoneNumberTypeTarget;
            if (rulePhoneNumberTypeTarget.PhoneNumberType.HasValue)
            {
                key = rulePhoneNumberTypeTarget.PhoneNumberType.Value;
                return true;
            }
            else
            {
                key = default(NormalizationPhoneNumberType);
                return false;
            }
        }

        public override Vanrise.Rules.BaseRuleStructureBehavior CreateNewBehaviorObject()
        {
            return new RuleBehaviorByNumberType();
        }
    }
}
