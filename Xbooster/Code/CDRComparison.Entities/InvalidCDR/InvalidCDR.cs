using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public class InvalidCDR
    {
        public string OriginalCDPN { get; set; }
        public string OriginalCGPN { get; set; }
        public string CDPN { get; set; }
        public string CGPN { get; set; }
        public DateTime Time { get; set; }
        public decimal DurationInSec { get; set; }
        public bool IsPartnerCDR { get; set; }
    }
}
