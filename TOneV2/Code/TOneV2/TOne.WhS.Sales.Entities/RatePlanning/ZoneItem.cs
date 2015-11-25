using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class ZoneItem
    {
        public long ZoneId { get; set; }

        public string ZoneName { get; set; }

        public long? CurrentRateId { get; set; }

        public Decimal? CurrentRate { get; set; }

        public DateTime? CurrentRateBED { get; set; }

        public DateTime? CurrentRateEED { get; set; }

        public bool? IsCurrentRateEditable { get; set; }

        public Decimal? NewRate { get; set; }

        public DateTime? NewRateBED { get; set; }

        public DateTime? NewRateEED { get; set; }

        public int? CurrentRoutingProductId { get; set; }

        public string CurrentRoutingProductName { get; set; }

        public DateTime? CurrentRoutingProductBED { get; set; }

        public DateTime? CurrentRoutingProductEED { get; set; }

        public bool? IsCurrentRoutingProductEditable { get; set; }

        public int? NewRoutingProductId { get; set; }

        public DateTime? NewRoutingProductBED { get; set; }

        public DateTime? NewRoutingProductEED { get; set; }
    }
}
