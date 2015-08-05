using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class TrafficStatisticSummaryInput
    {
        public TrafficStatisticFilter Filter { get; set; }

        public bool WithSummary { get; set; }

        public TrafficStatisticGroupKeys[] GroupKeys { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public List<string> Headers { get; set; }
    }
}
