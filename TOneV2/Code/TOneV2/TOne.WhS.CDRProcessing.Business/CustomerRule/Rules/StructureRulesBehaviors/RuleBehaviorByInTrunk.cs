using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;


namespace TOne.WhS.CDRProcessing.Business.CustomerRule.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByInTrunk : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<string>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<string> keys)
        {
            IRuleInTrunkCriteria ruleInTrunkCriteria = rule as IRuleInTrunkCriteria;
            keys = ruleInTrunkCriteria.InTrunks;
        }

        protected override bool TryGetKeyFromTarget(object target, out string key)
        {
            IRuleInTrunkTarget ruleInTrunkTarget = target as IRuleInTrunkTarget;
            if (ruleInTrunkTarget.InTrunk != null)
            {
                key = ruleInTrunkTarget.InTrunk;
                return true;
            }
            else
            {
                key = null;
                return false;
            }
        }
    }
}
