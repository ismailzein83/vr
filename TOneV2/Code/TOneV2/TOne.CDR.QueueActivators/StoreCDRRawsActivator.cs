using TOne.CDR.Data;
using TOne.CDR.Entities;
using Vanrise.Queueing.Entities;

namespace TOne.CDR.QueueActivators
{
    public class StoreCDRRawsActivator : QueueActivator
    {
        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            CDRBatch cdrBatch = item as CDRBatch;
            if (cdrBatch != null)
            {
                dataManager.ApplyCDRsToDB(dataManager.PrepareCDRsForDBApply(cdrBatch.CDRs, cdrBatch.SwitchId));
            }
        }
        public override void OnDisposed()
        {
            
        }
    }
}
