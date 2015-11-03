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

        List<string> IRuleInTrunkCriteria.InTrunks
        {
            get { return this.Criteria.InTrunks; }
        }

        List<string> IRuleInCarrierCriteria.InCarriers
        {
            get { return this.Criteria.InCarriers; }
        }

    }
}
