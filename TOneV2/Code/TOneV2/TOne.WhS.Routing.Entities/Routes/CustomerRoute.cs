using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class CustomerRoute : BaseRoute, ICompleteSupplierDataRoute
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string SaleZoneName { get; set; }

        public int VersionNumber { get; set; }

        public List<List<RouteOption>> GetAvailableRouteOptionsList()
        {
            return new List<List<RouteOption>>() { Options };
        }
    }

    public class CustomerRouteDefinition
    {
        public int CustomerId { get; set; }
        public string Code { get; set; }
        public long SaleZoneId { get; set; }
    }

    public class CustomerRouteData
    {
        public int CustomerId { get; set; }
        public string Code { get; set; }
        public long SaleZoneId { get; set; }
        public bool IsBlocked { get; set; }
        public int? ExecutedRuleId { get; set; }
        public string Options { get; set; }
        public int VersionNumber { get; set; }
        public string SupplierIds { get; set; }
    }
}
