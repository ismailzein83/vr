using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class AffectedRouteOptionRules
    {
        public List<RouteOptionRule> AddedRouteOptionRules { get; set; }
        public List<RouteOptionRule> UpdatedRouteOptionRules { get; set; }
        public List<RouteOptionRule> OpenedRouteOptionRules { get; set; }
        public List<RouteOptionRule> ClosedRouteOptionRules { get; set; }
    }
}
