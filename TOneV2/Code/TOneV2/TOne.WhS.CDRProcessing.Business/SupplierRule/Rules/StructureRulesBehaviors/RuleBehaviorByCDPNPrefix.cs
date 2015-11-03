using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Business.SupplierRule.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByCDPNPrefix : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByPrefix
    {
        protected override void GetPrefixesFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<string> prefixes)
        {
            IRuleSupplierCDPNPrefixCriteria ruleCDPNPrefixCriteria = rule as IRuleSupplierCDPNPrefixCriteria;
            prefixes = ruleCDPNPrefixCriteria.CDPNPrefixes;
        }

        protected override bool TryGetValueToCompareFromTarget(object target, out string value)
        {
            IRuleSupplierCDPNPrefixTarget ruleCDPNPrefixTarget = target as IRuleSupplierCDPNPrefixTarget;
            if (ruleCDPNPrefixTarget.CDPNPrefix != null)
            {
                value = ruleCDPNPrefixTarget.CDPNPrefix;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
    }
}
