using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class CustomerIdentificationRuleTarget : Vanrise.Rules.BaseRuleTarget, IRuleInCarrierTarget, IRuleInTrunkTarget, IRuleCDPNPrefixTarget
    {
        public string InCarrier { get; set; }

        public string InTrunk { get; set; }

        public string CDPNPrefix { get; set; }

    }
}
