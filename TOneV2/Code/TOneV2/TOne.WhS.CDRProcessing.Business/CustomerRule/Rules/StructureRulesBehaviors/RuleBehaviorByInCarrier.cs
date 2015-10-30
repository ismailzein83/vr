using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Business.CustomerRule.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByInCarrier : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<string>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<string> keys)
        {
            IRuleInCarrierCriteria ruleInCarrierCriteria = rule as IRuleInCarrierCriteria;
            keys = ruleInCarrierCriteria.IN_Carriers;
        }

        protected override bool TryGetKeyFromTarget(object target, out string key)
        {
            IRuleInCarrierTarget ruleInCarrierTarget = target as IRuleInCarrierTarget;
            if (ruleInCarrierTarget.IN_Carrier != null)
            {
                key = ruleInCarrierTarget.IN_Carrier;
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
