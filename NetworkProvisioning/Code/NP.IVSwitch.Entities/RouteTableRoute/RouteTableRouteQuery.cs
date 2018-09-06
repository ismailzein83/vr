using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities.RouteTableRoute
{
   public class RouteTableRouteQuery
    {
        public int RouteTableId { get; set; }
        public string ANumber { get; set; }
        public string BNumber { get; set; }
        public int Limit { get; set; }

    }
}
