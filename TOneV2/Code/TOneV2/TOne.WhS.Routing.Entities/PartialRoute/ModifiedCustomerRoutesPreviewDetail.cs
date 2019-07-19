using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class ModifiedCustomerRoutesPreviewDetail
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Code { get; set; }
        public string SaleZoneName { get; set; }
        public int? ExecutedRuleId { get; set; }
        public HashSet<int> SaleZoneServiceIds { get; set; }
        public Decimal? Rate { get; set; }
        public bool IsBlocked { get; set; }
        public List<ModifiedCustomerRouteOptionDetail> OrigRouteOptionDetails { get; set; }
        public List<ModifiedCustomerRouteOptionDetail> RouteOptionDetails { get; set; }
        public bool IsApproved { get; set; }
    }
}