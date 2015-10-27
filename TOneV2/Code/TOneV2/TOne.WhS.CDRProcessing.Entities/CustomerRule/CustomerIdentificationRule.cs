using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class CustomerIdentificationRule:BaseCarrierIdentificationRule
    {
        public CustomerIdentificationRuleCriteria Criteria { get; set; }
        public CustomerIdentificationRuleSettings Settings { get; set; }
    }
}
