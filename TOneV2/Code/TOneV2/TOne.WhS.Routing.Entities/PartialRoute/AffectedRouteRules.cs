using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class AffectedRouteRules
    {
        public List<RouteRule> AddedRouteRules { get; set; }
        public List<RouteRule> UpdatedRouteRules { get; set; }
        public List<RouteRule> OpenedRouteRules { get; set; }
        public List<RouteRule> ClosedRouteRules { get; set; }
    }
}
