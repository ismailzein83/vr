﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors
{
    public class RuleBehaviorByCustomer : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<int> keys)
        {
            IRuleCustomerCriteria ruleCustomerCriteria = rule as IRuleCustomerCriteria;
            keys = ruleCustomerCriteria.CustomerIds;
        }

        protected override bool TryGetKeyFromTarget(object target, out int key)
        {
            IRuleCustomerTarget ruleCustomerTarget = target as IRuleCustomerTarget;
            if (ruleCustomerTarget.CustomerId.HasValue)
            {
                key = ruleCustomerTarget.CustomerId.Value;
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
