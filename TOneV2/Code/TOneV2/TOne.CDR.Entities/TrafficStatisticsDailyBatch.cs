using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing;

namespace TOne.CDR.Entities
{
    public class TrafficStatisticsDailyBatch : PersistentQueueItem
    {
        public List<TABS.TrafficStats> TrafficStatisticsDaily { get; set; }

        public override string GenerateDescription()
        {
            return String.Format("Batch of {0} Traffic Statistics", TrafficStatisticsDaily.Count);
        }
    }
}
