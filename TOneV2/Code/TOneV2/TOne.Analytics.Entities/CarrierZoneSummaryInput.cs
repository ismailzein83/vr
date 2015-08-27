using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CarrierZoneSummaryInput
    {
        public CarrierZoneSummaryGenericFilter Filter { get; set; }

        public bool WithSummary { get; set; }

        public CarrierZoneSummaryStatsGroupKeys[] GroupKeys { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
