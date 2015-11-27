using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteQuery
    {
        public int RoutingDatabaseId { get; set; }

        public List<int> RoutingProductIds { get; set; }

        public List<int> SaleZoneIds { get; set; }
    }
}
