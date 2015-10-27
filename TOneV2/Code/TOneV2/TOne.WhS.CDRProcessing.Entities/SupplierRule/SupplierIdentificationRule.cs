using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class SupplierIdentificationRule:BaseCarrierIdentificationRule
    {
        public SupplierIdentificationRuleCriteria Criteria { get; set; }
        public SupplierIdentificationRuleSettings Settings { get; set; }
    }
}
