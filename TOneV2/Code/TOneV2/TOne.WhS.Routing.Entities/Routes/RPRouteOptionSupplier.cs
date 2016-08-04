using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOptionSupplier
    {
        static RPRouteOptionSupplier()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(RPRouteOptionSupplier),
                "SupplierId", "SupplierZones");
        }
        public int SupplierId { get; set; }

        public List<RPRouteOptionSupplierZone> SupplierZones { get; set; }
    }
}
