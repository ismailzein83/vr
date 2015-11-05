using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class CustomerZoneDetail
    {
        public int CustomerId { get; set; }

        public long SaleZoneId { get; set; }

        public int RoutingProductId { get; set; }

        public CustomerZoneRoutingProductSource RoutingProductSource { get; set; }
        
        public int SellingProductId { get; set; }

        public Decimal EffectiveRateValue { get; set; }

        public CustomerZoneRateSource RateSource { get; set; }
    }

    public class CustomerZoneDetailByZone : Dictionary<long, List<CustomerZoneDetail>>
    {

    }
}
