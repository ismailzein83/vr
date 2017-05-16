using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePricelistRPChangeDetail
    {
        public string ZoneName { get; set; }
        public string RoutingProductName { get; set; }
        public string RecentRoutingProductName { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public IEnumerable<int> RoutingProductServicesId { get; set; }
        public IEnumerable<int> RecentRouringProductServicesId { get; set; }

    }
}
