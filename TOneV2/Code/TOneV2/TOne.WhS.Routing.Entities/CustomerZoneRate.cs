using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class CustomerZoneRate
    {
        public int CustomerId { get; set; }

        public long SaleZoneId { get; set; }

        public int RoutingProductId { get; set; }

        public int? SellingProductId { get; set; }

        public Decimal Rate { get; set; }
    }

    public class CustomerZoneRatesByZone : Dictionary<long, List<CustomerZoneRate>>
    {

    }
}
