using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOptionSupplier
    {
        public int SupplierId { get; set; }

        public List<RPRouteOptionSupplierZone> SupplierZones { get; set; }
    }
}
