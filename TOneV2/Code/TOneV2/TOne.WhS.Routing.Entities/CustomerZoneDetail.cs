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

        public SaleEntityZoneRoutingProductSource RoutingProductSource { get; set; }
        
        public int SellingProductId { get; set; }

        public Decimal EffectiveRateValue { get; set; }

        public SalePriceListOwnerType RateSource { get; set; }

        public HashSet<int> CustomerServiceIds { get; set; } 
    }
     
    public class CustomerZoneDetailByZone : Dictionary<long, List<CustomerZoneDetail>>
    { 

    }

    public class CustomerZoneDetailBatch
    {
        public List<CustomerZoneDetail> CustomerZoneDetails { get; set; }
    }
}
