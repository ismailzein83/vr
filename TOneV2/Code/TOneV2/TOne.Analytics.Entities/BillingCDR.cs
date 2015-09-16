using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class BillingCDR
    {
        
        public long ID { get; set; }
        public String SwitchName { get; set; }
        public int SwitchID { get; set; }
        public String OurZoneName { get; set; }
        public int OurZoneID { get; set; }
        public String OriginatingZoneName { get; set; }
        public int OriginatingZoneID { get; set; }
        public DateTime Attempt { get; set; }
        public String CustomerInfo { get; set; }
        public String CustomerID { get; set; }
        public String CGPN { get; set; }
        public int PDD { get; set; }
        public String CDPN { get; set; }
        public String CDPNOut { get; set; }
        public Decimal DurationInSeconds { get; set; }
        public String ReleaseCode { get; set; }
        public String ReleaseSource { get; set; }
        public String SupplierID { get; set; }
        public String SupplierName{ get; set; }
        public string SupplierZoneName { get; set; }
        public int SupplierZoneID { get; set; }
        public long SwitchCdrID { get; set; }
        public String Tag { get; set; }
        public String Extra_Fields { get; set; }
        public DateTime Alert { get; set; }
        public DateTime Connect { get; set; }
        public string IsRerouted { get; set; }

    }
}
