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
    public class GenerateStatsActivator : QueueActivator
    {
        public override void OnDisposed()
        {
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            CDRBillingBatch cdrBillingBatch = item as CDRBillingBatch;
            TrafficStatisticByIntervalManager trafficStatisticByIntervalManager = new TrafficStatisticByIntervalManager(15);
            IEnumerable<TrafficStatisticByIntervalBatch> trafficStatisticBatches = trafficStatisticByIntervalManager.ConvertRawItemsToBatches(cdrBillingBatch.CDRs);
            if (trafficStatisticBatches != null && trafficStatisticBatches.Count()> 0)
            {
                foreach (TrafficStatisticByIntervalBatch trafficStatisticBatch in trafficStatisticBatches)
                {
                    outputItems.Add("Store Stats", trafficStatisticBatch);
                    outputItems.Add("Generate Daily Stats", trafficStatisticBatch);
                }
            }  

        }
    }
}
