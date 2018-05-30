using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class OverlappedZonesContext
    {
        public List<long> ZoneIds { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public int CarrierAccountId { get; set; }
        public int? DealId { get; set; }
    }
}
