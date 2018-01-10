using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class ZoneRoutingProductDetail
    {
        public ZoneRoutingProduct Entity { get; set; }
        public string ZoneName { get; set; }
        public string RoutingProductName { get; set; }
        public DateTime ZoneBED { get; set; }
        public DateTime? ZoneEED { get; set; }
    }
}
