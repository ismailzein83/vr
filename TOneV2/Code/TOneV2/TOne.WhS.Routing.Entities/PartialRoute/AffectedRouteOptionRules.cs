using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class AffectedRouteOptionRules
    {
        public AffectedRouteOptionRules()
        {
            AddedRouteOptionRules = new List<RouteOptionRule>();
            UpdatedRouteOptionRules = new List<RouteOptionRule>();
            OpenedRouteOptionRules = new List<RouteOptionRule>();
            ClosedRouteOptionRules = new List<RouteOptionRule>();
        }

        public List<RouteOptionRule> AddedRouteOptionRules { get; set; }
        public List<RouteOptionRule> UpdatedRouteOptionRules { get; set; }
        public List<RouteOptionRule> OpenedRouteOptionRules { get; set; }
        public List<RouteOptionRule> ClosedRouteOptionRules { get; set; }
    }

    public class AffectedRouteOptions
    {
        public IEnumerable<SupplierWithZones> SupplierWithZones { get; set; }

        public IEnumerable<int> CustomerIds { get; set; }

        public IEnumerable<long> ZoneIds { get; set; }

        public IEnumerable<CodeCriteria> Codes { get; set; }

        public RoutingExcludedDestinationData RoutingExcludedDestinationData { get; set; }  
    }
}
