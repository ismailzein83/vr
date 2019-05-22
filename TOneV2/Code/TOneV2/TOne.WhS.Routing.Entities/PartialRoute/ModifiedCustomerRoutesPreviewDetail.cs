using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class ModifiedCustomerRoutesPreviewDetail
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Code { get; set; }
        public string SaleZoneName { get; set; }
        public bool IsBlocked { get; set; }
        public List<ModifiedCustomerRouteOptionDetail> OrigRouteOptionDetails { get; set; }
        public List<ModifiedCustomerRouteOptionDetail> RouteOptionDetails { get; set; }
        public bool IsApproved { get; set; }
    }
}