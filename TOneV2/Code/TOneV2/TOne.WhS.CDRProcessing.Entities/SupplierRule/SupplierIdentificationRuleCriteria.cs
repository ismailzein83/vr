using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class SupplierIdentificationRuleCriteria
    {
        public List<string> OutTrunks { get; set; }
        public List<string> OutCarriers { get; set; }
        public List<string> CDPNPrefixes { get; set; }
     
    }
}
