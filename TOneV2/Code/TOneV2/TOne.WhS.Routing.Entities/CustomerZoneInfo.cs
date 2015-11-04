using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class CustomerZoneInfo
    {
        public int CustomerId { get; set; }

        public long SaleZoneId { get; set; }

        public CustomerZoneRoutingProduct RoutingProduct { get; set; }

        public CustomerZoneRate Rate { get; set; }

        public int SellingProductId { get; set; }

        public Decimal EffectiveRateValue { get; set; }
    }

    public class CustomerZoneInfoByZone : Dictionary<long, List<CustomerZoneInfo>>
    {

    }
}
