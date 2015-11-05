using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities.RatePlanning
{
    public class RoutingProductChanges : BaseChanges
    {
        public NewDefaultRoutingProduct NewDefaultRoutingProduct { get; set; }

        public DefaultRoutingProductChange ChangedDefaultRoutingProduct { get; set; }

        public List<NewZoneRoutingProduct> NewZoneRoutingProducts { get; set; }

        public List<ZoneRoutingProductChange> ChangedZoneRoutingProduct { get; set; }
    }

    public class NewZoneRoutingProduct
    {
        public long ZoneId { get; set; }

        public int RoutingProductId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class ZoneRoutingProductChange
    {
        public long ZoneRoutingProductId { get; set; }

        public DateTime? EED { get; set; }
    }

    public class NewDefaultRoutingProduct
    {
        public int RoutingProductId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class DefaultRoutingProductChange
    {
        public long DefaultRoutingProductId { get; set; }

        public DateTime? EED { get; set; }
    }
}
