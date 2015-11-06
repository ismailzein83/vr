using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class SupplierIdentificationRuleDetail
    {
        public SupplierIdentificationRule Entity { get; set; }
        public string CDPNPrefixes { get; set; }
        public string OutTrunks { get; set; }
        public string OutCarriers { get; set; }
        public string SupplierName { get; set; }
    }
}
