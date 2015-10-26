using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class SetCustomerRule:BaseSetCarrierRule
    {
        public SetCustomerRuleCriteria Criteria { get; set; }
        public SetCustomerRuleSettings Settings { get; set; }
    }
}
