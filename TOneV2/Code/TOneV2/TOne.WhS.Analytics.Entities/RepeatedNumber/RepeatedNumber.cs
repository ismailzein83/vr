using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public class RepeatedNumber
    {
        public long? SaleZoneId { get; set; }
        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }
        public int Attempt { get; set; }
        public decimal DurationInMinutes { get; set; }
        public String PhoneNumber { get; set; }
        public int SwitchId { get; set; }
    }
}
