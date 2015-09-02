using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
