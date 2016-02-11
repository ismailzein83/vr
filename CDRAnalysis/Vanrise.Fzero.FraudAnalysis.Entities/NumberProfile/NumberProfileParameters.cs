using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class NumberProfileParameters : INumberProfileParameters
    {
        public int GapBetweenConsecutiveCalls { get; set; }

        public int GapBetweenFailedConsecutiveCalls { get; set; }

        public int MaxLowDurationCall { get; set; }

        public int MinimumCountofCallsinActiveHour { get; set; }

        public HashSet<int> PeakHoursIds { get; set; }
    }
}
