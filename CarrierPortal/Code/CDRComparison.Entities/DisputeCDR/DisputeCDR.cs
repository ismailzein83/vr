using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public class DisputeCDR
    {
        public string SystemCDPN { get; set; }
        public string PartnerCDPN { get; set; }
        public string SystemCGPN { get; set; }
        public string PartnerCGPN { get; set; }
        public DateTime SystemTime { get; set; }
        public DateTime PartnerTime { get; set; }
        public Decimal SystemDurationInSec { get; set; }
        public Decimal PartnerDurationInSec { get; set; }
    }
}
