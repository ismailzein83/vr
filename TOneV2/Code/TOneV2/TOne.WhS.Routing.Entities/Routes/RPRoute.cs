using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class BaseRPRoute
    {
        public int RoutingProductId { get; set; }

        public long SaleZoneId { get; set; }

        public int SellingNumberPlanID { get; set; }

        public string SaleZoneName { get; set; }

        public HashSet<int> SaleZoneServiceIds { get; set; }

        public bool IsBlocked { get; set; }

        public int ExecutedRuleId { get; set; }

        public decimal? EffectiveRateValue { get; set; }

        public Dictionary<Guid, IEnumerable<RPRouteOption>> RPOptionsByPolicy { get; set; }
    }

    public class RPRoute : BaseRPRoute
    {
        public Dictionary<int, RPRouteOptionSupplier> OptionsDetailsBySupplier { get; set; }
    }

    public class RPRouteByCustomer
    {
        public int? CustomerId { get; set; }
        public List<RPRoute> RPRoutes { get; set; }
    }

    public class RPRouteBatch
    {
        public RPRouteBatch()
        {
            this.RPRouteByCustomers = new List<RPRouteByCustomer>();
        }

        public List<RPRouteByCustomer> RPRouteByCustomers { get; set; }
    }

    public class RPRouteByCode : BaseRoute
    {
        public int RoutingProductId { get; set; }
        public string SaleZoneName { get; set; }
        public int SellingNumberPlanID { get; set; }
        public List<SupplierZoneDetail> SupplierZoneDetails { get; set; }
        public Dictionary<long, string> SupplierCodeMatchByZoneId { get; set; }
    }
}