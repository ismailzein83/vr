using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace TOne.WhS.CDRProcessing.QueueActivators
{
    public class StoreDailyStatsActivator : QueueActivator
    {
        public override void OnDisposed()
        {
            throw new NotImplementedException();
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            throw new NotImplementedException();
        }
    }
}
