using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOptionSupplierDetail
    {
        public string SupplierName { get; set; }

        public IEnumerable<RPRouteOptionSupplierZoneDetail> SupplierZones { get; set; }
    }
}
