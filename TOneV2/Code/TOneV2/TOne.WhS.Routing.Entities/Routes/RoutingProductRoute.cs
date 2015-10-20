using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RoutingProductRoute
    {
        public int RoutingProductId { get; set; }

        public long SaleZoneId { get; set; }

        public bool IsBlocked { get; set; }

        public int ExecutedRuleId { get; set; }

        public List<RoutingProductRouteOption> Options { get; set; }
    }
}
