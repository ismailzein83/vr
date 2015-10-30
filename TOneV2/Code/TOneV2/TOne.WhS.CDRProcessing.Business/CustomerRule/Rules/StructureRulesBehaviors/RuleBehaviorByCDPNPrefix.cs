using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Business.CustomerRule.Rules.StructureRulesBehaviors
{
    public class RuleBehaviorByCDPNPrefix : Vanrise.Rules.RuleStructureBehaviors.RuleStructureBehaviorByKey<string>
    {
        protected override void GetKeysFromRule(Vanrise.Rules.BaseRule rule, out IEnumerable<string> keys)
        {
            IRuleCDPNPrefixCriteria ruleCDPNPrefixCriteria = rule as IRuleCDPNPrefixCriteria;
            keys = ruleCDPNPrefixCriteria.CDPNPrefixes;
        }

        protected override bool TryGetKeyFromTarget(object target, out string key)
        {
            IRuleCDPNPrefixTarget ruleCDPNPrefixTarget = target as IRuleCDPNPrefixTarget;
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
