using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteDetail
    {
        public RPRoute Entity { get; set; }

        public string RoutingProductName { get; set; }

        public string SaleZoneName { get; set; }

        public IEnumerable<RPRouteOptionDetail> RouteOptionsDetails { get; set; }
    }
}
