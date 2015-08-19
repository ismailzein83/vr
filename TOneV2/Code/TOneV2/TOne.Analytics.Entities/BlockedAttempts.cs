using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class BlockedAttempts
    {
        public int OurZoneID { get; set; }
        public string OurZoneName { get; set; }
        public int BlockAttempt { get; set; }
        public string ReleaseCode { get; set; }
        public Byte SwitchID { get; set; }
        public string SwitchName { get; set; }
        public string ReleaseSource { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName{ get; set; }
        public DateTime FirstCall { get; set; }
        public DateTime LastCall { get; set; }
        public string PhoneNumber { get; set; }
        public string CLI { get; set; }

    }
}
