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

        public List<string> CDPNPrefixes
        {
            get { return this.Criteria.CDPNPrefixes; }
        }

        public List<string> IN_Trunks
        {
            get { return this.Criteria.IN_Trunks; }
        }

        public List<string> IN_Carriers
        {
            get { return this.Criteria.IN_Carriers; }
        }

    }
}
