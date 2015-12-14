using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class NewZoneRoutingProduct
    {
        public long ZoneId { get; set; }

        public int ZoneRoutingProductId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class ZoneRoutingProductChange
    {
        public long ZoneId { get; set; }

        public int ZoneRoutingProductId { get; set; }

        public DateTime? EED { get; set; }
    }

    public class NewDefaultRoutingProduct
    {
        public int DefaultRoutingProductId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class DefaultRoutingProductChange
    {
        public int DefaultRoutingProductId { get; set; }

        public DateTime? EED { get; set; }
    }
}
