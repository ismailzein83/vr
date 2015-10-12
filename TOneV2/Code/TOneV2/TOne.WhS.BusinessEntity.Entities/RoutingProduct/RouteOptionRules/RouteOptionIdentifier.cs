using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteOptionIdentifier : RouteIdentifier
    {
        public int SupplierId { get; set; }

        public long SupplierZoneId { get; set; }
    }
}
