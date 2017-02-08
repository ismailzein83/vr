using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class ZoneRoutingProduct
    {
        public long ZoneId { get; set; }
        public int ZoneRoutingProductId { get; set; }
        public List<int> ServiceIds { get; set; }
        public bool IsInherited { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}
