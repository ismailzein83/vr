using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class TrafficStatisticSummaryBigResult : BigResult<TrafficStatisticGroupSummary>
    {
        public TrafficStatistic Summary { get; set; }
    }
}
