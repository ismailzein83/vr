using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class TrafficSummaryView
    {
        public decimal Sales { get; set; }
        public decimal Purchases { get; set; }
        public decimal Profit { get; set; }
        public int NumberOfCalls { get; set; }
        public decimal DurationInMinutes { get; set; }

    }
}
