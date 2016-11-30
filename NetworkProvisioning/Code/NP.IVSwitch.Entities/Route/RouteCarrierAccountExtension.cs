using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities 
{
    public class RouteCarrierAccountExtension
    {
        public List<RouteInfo> RouteInfo { get; set; }

    }

    public class RouteInfo
    {
        public int RouteId { get; set; }

        public int Percentage { get; set; }
    }
}
