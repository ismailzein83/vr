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
    public class StoreBillingStatsActivator : QueueActivator
    {
        public override void OnDisposed()
        {
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            BillingStatisticBatch trafficStatisticBatch = item as BillingStatisticBatch;
            BillingStatisticManager trafficStatisticBillingManager = new BillingStatisticManager();
            trafficStatisticBillingManager.UpdateStatisticBatch(trafficStatisticBatch);
        }
    }
}
