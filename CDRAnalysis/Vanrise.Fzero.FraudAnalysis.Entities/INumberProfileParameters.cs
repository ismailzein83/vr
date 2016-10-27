using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public interface INumberProfileParameters
    {
        int GapBetweenConsecutiveCalls { get; set; }
        int GapBetweenFailedConsecutiveCalls { get; set; }
        int MaxLowDurationCall { get; set; }
        int MinimumCountofCallsinActiveHour { get; set; }
        HashSet<int> PeakHoursIds { get; set; }
    }
 
}
