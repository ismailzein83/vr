using System;
using TOne.CDR.Data;
using TOne.CDR.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.CDR.QueueActivators
{
    public class StoreMainCDRsActivator : QueueActivator
    {

        public override void OnDisposed()
        {
            
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();

            CDRMainBatch cdr = item as CDRMainBatch;

            if (cdr == null || cdr.MainCDRs == null) return;

            Object preparedMainCdRs = dataManager.PrepareMainCDRsForDBApply(cdr.MainCDRs);
            dataManager.ApplyMainCDRsToDB(preparedMainCdRs);

        }
    }
}
