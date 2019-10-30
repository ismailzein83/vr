using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public class DealCapacity
    {
        public DateTime FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public List<int> DealIds { get; set; }
        public double ExpectedCapacity { get; set; }
        public double AllowedCapacity { get; set; }// allowed capacity for carrier in intersected period = Nominal Capacity * intersected period
        public string DealNames { get; set; }
    }
}
