﻿using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Business.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByNumberLength : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<int> keys)
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
    }
}
