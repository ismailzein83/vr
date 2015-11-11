using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Business;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.WhS.CDRProcessing.QueueActivators
{
    public class GenerateDailyStatsActivator : QueueActivator
    {
        public override void OnDisposed()
        {
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            TrafficStatisticByIntervalBatch trafficStatisticByIntervalBatch = item as TrafficStatisticByIntervalBatch;
            TrafficStatisticDailyManager trafficStatisticDailyManager = new TrafficStatisticDailyManager();
            IEnumerable<TrafficStatisticDailyBatch> trafficStatisticBatches = trafficStatisticDailyManager.ConvertRawItemsToBatches(trafficStatisticByIntervalBatch.ItemsByKey.Values);
            if (trafficStatisticBatches != null && trafficStatisticBatches.Count()>0)
            foreach (TrafficStatisticDailyBatch trafficStatisticBatch in trafficStatisticBatches)
                outputItems.Add("Store Daily Stats", trafficStatisticBatch);
        }
    }
}
