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
    public class StoreCDRMainActivator : QueueActivator
    {
        public override void OnDisposed()
        {
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            CDRMainBatch cdrBatch = item as CDRMainBatch;
            CDRMainManager manager = new CDRMainManager();
            manager.SaveCDRMainBatchToDB(cdrBatch);
        }
    }
}
