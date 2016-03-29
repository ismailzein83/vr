using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public class CDR
    {
        public string CDPN { get; set; }

        public string CGPN { get; set; }

        public DateTime Time { get; set; }

        public Decimal DurationInSec { get; set; }
        public bool IsPartnerCDR { get; set; }

        public Dictionary<string, object> ExtraFields { get; set; }
    }
}
