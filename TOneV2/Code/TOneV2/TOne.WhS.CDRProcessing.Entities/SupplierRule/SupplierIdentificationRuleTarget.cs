using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class SupplierIdentificationRuleTarget : Vanrise.Rules.BaseRuleTarget, IRuleOutCarrierTarget, IRuleOutTrunkTarget, IRuleSupplierCDPNPrefixTarget
    {
        public string OutCarrier { get; set; }

        public string OutTrunk { get; set; }

        public string CDPNPrefix { get; set; }
    }
}
