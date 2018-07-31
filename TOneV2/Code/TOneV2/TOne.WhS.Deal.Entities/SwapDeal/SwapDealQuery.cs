using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealQuery 
    {
        public string Name { get; set; }
        public List<int> CarrierAccountIds { get; set; }
        public List<DealStatus> Status { get; set; }
    }
}
