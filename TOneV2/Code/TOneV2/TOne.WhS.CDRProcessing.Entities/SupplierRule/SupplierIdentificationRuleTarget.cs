using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class SupplierIdentificationRuleTarget : Vanrise.Rules.BaseRuleTarget, IRuleOutCarrierTarget, IRuleOutTrunkTarget, IRuleSupplierCDPNPrefixTarget
    {
        public string OUT_Carrier { get; set; }

        public string OUT_Trunk { get; set; }

        public string CDPNPrefix { get; set; }
    }
}
