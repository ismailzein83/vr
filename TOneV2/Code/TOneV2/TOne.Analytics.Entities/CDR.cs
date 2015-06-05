using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CDR
    {
        public long ID { get; set; }
        public DateTime Attempt { get; set; }
        public DateTime Alert { get; set; }
        public DateTime Connect { get; set; }
        public DateTime Disconnect { get; set; }
        public int DurationInSeconds { get; set; }
        public String CustomerID { get; set; }
        public int OurZoneID { get; set; }
        public int OriginatingZoneID { get; set; }
        public String SupplierID { get; set; }
        public int SupplierZoneID { get; set; }
        public String CDPN { get; set; }
        public String CGPN { get; set; }
        public String ReleaseCode { get; set; }
        public String ReleaseSource { get; set; }
        public int SwitchID { get; set; }
        public long SwitchCdrID { get; set; }
        public String Tag { get; set; }
        public String Extra_Fields { get; set; }
        public String Port_IN { get; set; }
        public String Port_OUT { get; set; }
        public String OurCode { get; set; }
        public String SupplierCode { get; set; }
        public String CDPNOut { get; set; }
        public long SubscriberID { get; set; }
        public String SIP { get; set; }

    }
}
