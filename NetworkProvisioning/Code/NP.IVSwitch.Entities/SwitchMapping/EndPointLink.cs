using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class EndPointLink
    {
        public int CarrierAccountId { get ; set;}

        public List<int> EndPointIds { get; set; }
    }
}
