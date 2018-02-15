using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOptionSupplierZoneDetail
    {
        public RPRouteOptionSupplierZone Entity { get; set; }
        public string SupplierZoneName { get; set; }
        public decimal ConvertedSupplierRate { get; set; }
        public decimal? FutureRate { get; set; }
        public DateTime? RateEED { get; set; }
        public RouteOptionEvaluatedStatus? EvaluatedStatus { get; set; } 
    }
}
