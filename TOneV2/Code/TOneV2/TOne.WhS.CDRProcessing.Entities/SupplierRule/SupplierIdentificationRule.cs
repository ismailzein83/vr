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

        List<string> IRuleSupplierCDPNPrefixCriteria.CDPNPrefixes
        {
            get { return this.Criteria.CDPNPrefixes; }
        }

        List<string> IRuleOutTrunkCriteria.OutTrunks
        {
            get { return this.Criteria.OutTrunks; }
        }

        List<string> IRuleOutCarrierCriteria.OutCarriers
        {
            get { return this.Criteria.OutCarriers; }
        }
    }
}
