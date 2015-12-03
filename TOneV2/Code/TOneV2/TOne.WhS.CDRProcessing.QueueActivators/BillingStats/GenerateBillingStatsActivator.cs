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
    public class GenerateBillingStatsActivator : QueueActivator
    {
        public override void OnDisposed()
        {
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            CDRMainBatch cdrBillingBatch = item as CDRMainBatch;
            BillingStatisticManager trafficStatisticBillingManager = new BillingStatisticManager();
            IEnumerable<BillingStatisticBatch> trafficStatisticBatches = trafficStatisticBillingManager.ConvertRawItemsToBatches(cdrBillingBatch.MainCDRs);
            if (trafficStatisticBatches != null && trafficStatisticBatches.Count() > 0)
            {
                foreach (BillingStatisticBatch trafficStatisticBillingBatch in trafficStatisticBatches)
                {
                    outputItems.Add("Store BillingStats", trafficStatisticBillingBatch);
                }
            }

        }
    }
}
