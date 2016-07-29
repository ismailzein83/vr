using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Entities
{
    public class SwitchInfo
    {
        public string SwitchId { get; set; }

        public string Name { get; set; }

        public SwitchRouteSynchronizer RouteSynchronizer { get; set; }
    }
}
