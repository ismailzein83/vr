using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class CustomerRouteDetail
    {
        public CustomerRoute Entity { get; set; }

        public string CustomerName { get; set; }

        public string ZoneName { get; set; }

        public List<CustomerRouteOptionDetail> RouteOptionDetails { get; set; }
        
    }

    public class CustomerRouteOptionDetail
    {
        public string SupplierName { get; set; }

        public string SupplierCode { get; set; }

        public string SupplierZoneName { get; set; }

        public Decimal SupplierRate { get; set; }

        public Decimal? Percentage { get; set; }

        public bool IsBlocked { get; set; }

        public List<int> ExactSupplierServiceIds { get; set; } 
    }
}
