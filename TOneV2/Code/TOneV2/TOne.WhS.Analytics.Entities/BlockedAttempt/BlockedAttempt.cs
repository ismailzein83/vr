using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public class BlockedAttempt
    {
        public int CustomerID { get; set; }
        public long SaleZoneID { get; set; }
        public int BlockedAttempts { get; set; }
        public String ReleaseCode { get; set; }
        public String ReleaseSource { get; set; }
        public DateTime? FirstAttempt { get; set; }
        public DateTime? LastAttempt { get; set; }
        public String CDPN { get; set; }
        public String CGPN { get; set; }
    }
}
