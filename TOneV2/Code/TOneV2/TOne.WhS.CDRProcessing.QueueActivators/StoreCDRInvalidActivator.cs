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
    public class StoreCDRInvalidActivator : QueueActivator
    {
        public override void OnDisposed()
        {
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            CDRInvalidBatch cdrBatch = item as CDRInvalidBatch;
            CDRInvalidManager manager = new CDRInvalidManager();
            manager.SaveInvalidCDRBatchToDB(cdrBatch);
        }
    }
}
