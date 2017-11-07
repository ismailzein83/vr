using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class AffectedRouteRules
    {
        public List<RouteRule> AddedRouteRules { get; set; }
        public List<RouteRule> UpdatedRouteRules { get; set; }
        public List<RouteRule> OpenedRouteRules { get; set; }
        public List<RouteRule> ClosedRouteRules { get; set; }
    }

    public class AffectedRoutes
    {
        public IEnumerable<int> CustomerIds { get; set; }

        public IEnumerable<long> ZoneIds { get; set; }

        public IEnumerable<CodeCriteria> Codes { get; set; }

        public List<string> ExcludedCodes { get; set; }
    }
}
