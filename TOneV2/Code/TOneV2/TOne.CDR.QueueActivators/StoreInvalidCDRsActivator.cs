using System;
using TOne.CDR.Data;
using TOne.CDR.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.CDR.QueueActivators
{
    public class StoreInvalidCDRsActivator : QueueActivator
    {

        public override void OnDisposed()
        {
            
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            ICDRInvalidDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRInvalidDataManager>();

            CDRInvalidBatch cdr = item as CDRInvalidBatch;

            if (cdr == null || cdr.InvalidCDRs == null) return;
            dataManager.SaveInvalidCDRsToDB(cdr.InvalidCDRs);
        }
    }
}
