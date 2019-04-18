using System;
using System.Collections.Generic;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business.RouteRules.StructureRuleBehaviors
{
    public class RuleBehaviorByDeal : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.IVRRule rule, out IEnumerable<int> keys)
        {
            IRuleDealCriteria ruleDealCriteria = rule as IRuleDealCriteria;
            keys = ruleDealCriteria.DealIds;
        }

        protected override bool TryGetKeyFromTarget(object target, out int key)
        {
            IRuleDealTarget ruleDealTarget = target as IRuleDealTarget;
            if (ruleDealTarget.DealId.HasValue)
            {
                key = ruleDealTarget.DealId.Value;
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
            return new RuleBehaviorByDeal();
        }
    }
}