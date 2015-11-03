using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Business.CustomerRule.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByCDPNPrefix : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByPrefix
    {
        protected override void GetPrefixesFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<string> prefixes)
        {
            IRuleCDPNPrefixCriteria ruleCDPNPrefixCriteria = rule as IRuleCDPNPrefixCriteria;
            prefixes = ruleCDPNPrefixCriteria.CDPNPrefixes;
        }

        protected override bool TryGetValueToCompareFromTarget(object target, out string value)
        {
            IRuleCDPNPrefixTarget ruleCDPNPrefixTarget = target as IRuleCDPNPrefixTarget;
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
