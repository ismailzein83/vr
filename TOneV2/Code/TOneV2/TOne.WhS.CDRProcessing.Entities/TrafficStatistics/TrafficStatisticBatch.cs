using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class TrafficStatisticBatch<T> :PersistentQueueItem, Vanrise.Entities.StatisticManagement.IStatisticBatch<T>
        where T : BaseTrafficStatistic
    {
          
        public DateTime BatchStart { get; set; }

        public DateTime BatchEnd { get; set; }

        public Dictionary<string, T> ItemsByKey { get; set; }

        public override string GenerateDescription()
        {
            return String.Format("TrafficStatsBatch of {0} TrafficStats", ItemsByKey.Count());
        }
    }
}
