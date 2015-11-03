using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Business.SupplierRule.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByOutTrunk : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<string>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<string> keys)
        {
            IRuleOutTrunkCriteria ruleOutTrunkCriteria = rule as IRuleOutTrunkCriteria;
            keys = ruleOutTrunkCriteria.OutTrunks;
        }

        protected override bool TryGetKeyFromTarget(object target, out string key)
        {
            IRuleOutTrunkTarget ruleOutTrunkTarget = target as IRuleOutTrunkTarget;
            if (ruleOutTrunkTarget.OutTrunk != null)
            {
                key = ruleOutTrunkTarget.OutTrunk;
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
