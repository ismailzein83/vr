using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class SetSupplierRuleCriteria
    {
        public List<int> Out_Trunk { get; set; }
        public List<int> Out_Carrier { get; set; }
        public List<int> CDPN { get; set; }
     
    }
}
