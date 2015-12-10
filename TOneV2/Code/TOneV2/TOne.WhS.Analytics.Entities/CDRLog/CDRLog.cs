using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public class CDRLog
    {
        public long ID { get; set; }   
        public int SwitchID { get; set; }
        public long SaleZoneID { get; set; }
        public int OriginatingZoneID { get; set; }
        public DateTime Attempt { get; set; }
        public int CustomerID { get; set; }
        public String CGPN { get; set; }
        public int PDD { get; set; }
        public String CDPN { get; set; }
        public String CDPNOut { get; set; }
        public int DurationInSeconds { get; set; }
        public String ReleaseCode { get; set; }
        public String ReleaseSource { get; set; }
        public int SupplierID { get; set; }
        public long SupplierZoneID { get; set; }
        public long SwitchCdrID { get; set; }
        public String Tag { get; set; }
        public String Extra_Fields { get; set; }
        public DateTime Alert { get; set; }
        public DateTime Connect { get; set; }
        public string IsRerouted { get; set; }
    }
}
