using System.Collections.Generic;

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
