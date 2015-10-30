using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Business.SupplierRule.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByCDPNPrefix : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<string>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<string> keys)
        {
            IRuleSupplierCDPNPrefixCriteria ruleCDPNPrefixCriteria = rule as IRuleSupplierCDPNPrefixCriteria;
            keys = ruleCDPNPrefixCriteria.CDPNPrefixes;
        }

        protected override bool TryGetKeyFromTarget(object target, out string key)
        {
            IRuleSupplierCDPNPrefixTarget ruleCDPNPrefixTarget = target as IRuleSupplierCDPNPrefixTarget;
            if (ruleCDPNPrefixTarget.CDPNPrefix != null)
            {
                key = ruleCDPNPrefixTarget.CDPNPrefix;
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
