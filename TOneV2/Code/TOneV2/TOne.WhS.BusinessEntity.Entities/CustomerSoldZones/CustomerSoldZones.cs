using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomersSoldZone
    {
        public long ZoneId { get; set; }
        public List<CustomerZoneData> CustomerZoneData { get; set; }
        public int SaleCount { get; set; }
        public  DateTime EffectiveOn { get; set; }
    }

    public class CustomerZoneData
    {
        public int CustomerId { get; set; }

        public decimal Rate { get; set; }

        public int RoutingProductId { get; set; }

        public HashSet<int> Services { get; set; }
    }

    public class CustomersSoldZoneDetail
    {
        public long ZoneId { get; set; }
        public string Name { get; set; }
        public IEnumerable<CustomerZoneDataDetail> CustomerZones { get; set; }
        public int SaleCount { get; set; }
        public DateTime EffectiveOn { get; set; }

    }

    public class CustomerZoneDataDetail
    { 
        public string CustomerName { get; set; }

        public decimal Rate { get; set; }
        
        public int RoutingProductId { get; set; }

        public HashSet<int> Services { get; set; }
    }
}
