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
    public class StoreRawCDRActivator: QueueActivator
    {
          public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            CDRBatch cdrBatch = item as CDRBatch;
            CDRManager manager = new CDRManager();
            manager.SaveCDRBatchToDB(cdrBatch);
        }
        public override void OnDisposed()
        {
            
        }
    }
}
