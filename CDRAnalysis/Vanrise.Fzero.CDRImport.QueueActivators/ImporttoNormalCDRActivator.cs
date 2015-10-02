using Vanrise.Fzero.CDRImport.Data;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.CDRImport.QueueActivators
{
    public class ImporttoNormalCDRActivator : QueueActivator
    {
        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            ImportedCDRBatch cdrBatch = (ImportedCDRBatch)item;
            
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            dataManager.SaveCDRsToDB(cdrBatch.CDRs);
        }

        public override void OnDisposed()
        {
            
        }

    }
}
