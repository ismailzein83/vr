using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities 
{
    public class EndPointCarrierAccountExtension
    {
        public List<EndPointInfo> AclEndPointInfo { get; set; }
        public List<EndPointInfo> UserEndPointInfo { get; set; }

    }

    public class EndPointInfo
    {
        public int EndPointId { get; set; }        
    }
}
