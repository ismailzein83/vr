using Vanrise.Fzero.CDRImport.Data;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.CDRImport.Business
{
    public class StagingCDRImportActivator : QueueActivator
    {
        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            ImportedStagingCDRBatch cdrBatch = (ImportedStagingCDRBatch)item;

            IStagingCDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<IStagingCDRDataManager>();
            dataManager.SaveStagingCDRsToDB(cdrBatch.StagingCDRs);
        }

        public override void OnDisposed()
        {
            
        }

    }
}
