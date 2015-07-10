using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CarrierProfileReport
    {
    
        public string Zone { get; set; }
        public int Month { get; set; }
        public string MonthYear { get; set; }

        public decimal? Durations { get; set; }

        public double? Amount { get; set; }
    }
}
