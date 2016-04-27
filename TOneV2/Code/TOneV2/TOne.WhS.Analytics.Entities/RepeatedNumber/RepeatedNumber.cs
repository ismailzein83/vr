using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public class RepeatedNumber
    {
        public long Id { get; set; }
        public DateTime Attempt { get; set; }
        public DateTime Alert { get; set; }
        public DateTime Connect { get; set; }
        public DateTime DisConnect { get; set; }
        public int DurationInSeconds { get; set; }
        public int CustomerId { get; set; }
        public int OurZoneId { get; set; }
        public int OriginatingZoneId { get; set; }
        public int SupplierId { get; set; }
        public long SupplierZoneId { get; set; }
        public String Cdpn { get; set; }
        public String Cgpn { get; set; }
        public int SwitchId { get; set; }
    }
}
