using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class CustomerIdentificationRule : BaseCarrierIdentificationRule, IRuleCDPNPrefixCriteria, IRuleInTrunkCriteria, IRuleInCarrierCriteria
    {
        public CustomerIdentificationRuleCriteria Criteria { get; set; }
        public CustomerIdentificationRuleSettings Settings { get; set; }

        List<string> IRuleCDPNPrefixCriteria.CDPNPrefixes
        {
            get { return this.Criteria.CDPNPrefixes; }
        }

        List<string> IRuleInTrunkCriteria.IN_Trunks
        {
            get { return this.Criteria.IN_Trunks; }
        }

        List<string> IRuleInCarrierCriteria.IN_Carriers
        {
            get { return this.Criteria.IN_Carriers; }
        }

    }
}
