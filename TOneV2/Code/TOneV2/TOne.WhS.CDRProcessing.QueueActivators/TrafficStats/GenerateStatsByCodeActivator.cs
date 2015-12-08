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
    public class GenerateStatsByCodeActivator : QueueActivator
    {
        public override void OnDisposed()
        {
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            CDRBillingBatch cdrBillingBatch = item as CDRBillingBatch;
            TrafficStatisticByCodeManager trafficStatisticByCodeManager = new TrafficStatisticByCodeManager(15);
            IEnumerable<TrafficStatisticByCodeBatch> trafficStatisticBatches = trafficStatisticByCodeManager.ConvertRawItemsToBatches(cdrBillingBatch.CDRs);
            if (trafficStatisticBatches != null && trafficStatisticBatches.Count() > 0)
            {
                foreach (TrafficStatisticByCodeBatch trafficStatisticBatch in trafficStatisticBatches)
                {
                    outputItems.Add("Store StatsByCode", trafficStatisticBatch);
                }
            }  
        }
    }
}
