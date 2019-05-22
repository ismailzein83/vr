using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class ModifiedCustomerRoutesPreview : ICompleteSupplierDataRoute
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public String CustomerName { get; set; }
        public String Code { get; set; }
        public long SaleZoneId { get; set; }
        public String SaleZoneName { get; set; }
        public bool OrigIsBlocked { get; set; }
        public bool IsBlocked { get; set; }
        public int? OrigExecutedRuleId { get; set; }
        public int? ExecutedRuleId { get; set; }
        public List<RouteOption> OrigRouteOptions { get; set; }
        public List<RouteOption> RouteOptions { get; set; }
        public string SupplierIds { get; set; }
        public bool IsApproved { get; set; }

        public List<List<RouteOption>> GetAvailableRouteOptionsList()
        {
            return new List<List<RouteOption>>() { OrigRouteOptions, RouteOptions };
        }
    }
}
