using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class SupplierIdentificationRule : BaseCarrierIdentificationRule, IRuleSupplierCDPNPrefixCriteria, IRuleOutTrunkCriteria, IRuleOutCarrierCriteria
    {
        public SupplierIdentificationRuleCriteria Criteria { get; set; }
        public SupplierIdentificationRuleSettings Settings { get; set; }

        public List<string> CDPNPrefixes
        {
            get { return this.Criteria.CDPNPrefixes; }
        }

        public List<string> OUT_Trunks
        {
            get { return this.Criteria.Out_Trunks; }
        }

        public List<string> OUT_Carriers
        {
            get { return this.Criteria.Out_Carriers; }
        }
    }
}
