using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class SupplierIdentificationRuleCriteria
    {
        public List<string> Out_Trunks { get; set; }
        public List<string> Out_Carriers { get; set; }
        public List<string> CDPNPrefixes { get; set; }
     
    }
}
