using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class SetCustomerRuleCriteria
    {
        public List<int> IN_Trunk { get; set; }
        public List<int> IN_Carrier { get; set; }
        public List<int> CDPN { get; set; }
      
    }
}
