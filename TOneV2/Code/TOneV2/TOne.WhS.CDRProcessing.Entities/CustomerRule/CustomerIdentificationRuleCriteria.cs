using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class CustomerIdentificationRuleCriteria
    {
        public List<string> InTrunks { get; set; }
        public List<string> InCarriers { get; set; }
        public List<string> CDPNPrefixes { get; set; }
      
    }
}
