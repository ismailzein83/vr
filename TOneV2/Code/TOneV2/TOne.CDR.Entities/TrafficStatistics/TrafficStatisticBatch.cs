using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing;

namespace TOne.CDR.Entities
{
    public class TrafficStatisticBatch : PersistentQueueItem
    {
        public TrafficStatisticsByKey TrafficStatistics { get; set; }

        public DateTime BatchStart { get; set; }

        public DateTime BatchEnd { get; set; }

        public override string GenerateDescription()
        {
            return String.Format("Batch of {0} Traffic Statistics", TrafficStatistics.Count);
        }
    }
}