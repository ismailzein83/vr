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

        public List<string> OutTrunks
        {
            get { return this.Criteria.OutTrunks; }
        }

        public List<string> OutCarriers
        {
            get { return this.Criteria.OutCarriers; }
        }
    }
}
