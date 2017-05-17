using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePricelistRPChange
    {
        public int CountryId { get; set; }
        public int PriceListId { get; set; }
        public string ZoneName { get; set; }
        public long? ZoneId { get; set; }
        public int RoutingProductId { get; set; }

        public int? RecentRoutingProductId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
