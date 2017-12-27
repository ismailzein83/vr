using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum SaleEntityZoneRoutingProductSource : byte { CustomerZone, CustomerDefault, ProductZone, ProductDefault }

    public class SaleEntityZoneRoutingProduct
    {
        public long SaleEntityZoneRoutingProductId { get; set; }
        public int RoutingProductId { get; set; }

        public SaleEntityZoneRoutingProductSource Source { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
