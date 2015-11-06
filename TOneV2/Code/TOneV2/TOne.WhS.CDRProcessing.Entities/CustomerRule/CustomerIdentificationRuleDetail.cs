using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class CustomerIdentificationRuleDetail
    {
        public CustomerIdentificationRule Entity { get; set; }
        public string CDPNPrefixes { get; set; }
        public string InTrunks { get; set; }
        public string InCarriers { get; set; }
        public string CustomerName { get; set; }
    }
}
