using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities 
{
    public class EndPointInfoFilter
    {
        public int? AssignableToCarrierAccountId { get; set; }
        public List<int> CustomerIds { get; set; }

    }
}
