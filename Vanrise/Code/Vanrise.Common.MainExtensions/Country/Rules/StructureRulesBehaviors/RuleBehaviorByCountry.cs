using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.Country.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByCountry : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<int>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.IVRRule rule, out IEnumerable<int> keys)
        {
            IRuleCountryCriteria ruleCountryCriteria = rule as IRuleCountryCriteria;
            keys = ruleCountryCriteria.CountryIds;
        }

        protected override bool TryGetKeyFromTarget(object target, out int key)
        {
            IRuleCountryTarget ruleCountryTarget = target as IRuleCountryTarget;
            if (ruleCountryTarget.CountryId.HasValue)
            {
                key = ruleCountryTarget.CountryId.Value;
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
            return new RuleBehaviorByCountry();
        }
    }
}
