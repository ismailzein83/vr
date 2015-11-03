using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Business.SupplierRule.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByOutCarrier : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<string>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<string> keys)
        {
            IRuleOutCarrierCriteria ruleOutCarrierCriteria = rule as IRuleOutCarrierCriteria;
            keys = ruleOutCarrierCriteria.OutCarriers;
        }

        protected override bool TryGetKeyFromTarget(object target, out string key)
        {
            IRuleOutCarrierTarget ruleOutCarrierTarget = target as IRuleOutCarrierTarget;
            if (ruleOutCarrierTarget.OutCarrier != null)
            {
                key = ruleOutCarrierTarget.OutCarrier;
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
