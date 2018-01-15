using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RouteOptionOrderExecutionContext : IRouteOptionOrderExecutionContext
    {
        public IEnumerable<IRouteOptionOrderTarget> Options { get; set; }

        public OrderDirection OrderDitection { get; set; }

        public RoutingDatabase RoutingDatabase { get; set; }
    }
}