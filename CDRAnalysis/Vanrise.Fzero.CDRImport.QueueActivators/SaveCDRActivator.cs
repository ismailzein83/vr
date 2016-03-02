using System;
using Vanrise.Fzero.CDRImport.Data;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.CDRImport.QueueActivators
{
    public class SaveCDRActivator : QueueActivator
    {
        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            throw new NotImplementedException();            
        }

        public override void OnDisposed()
        {
            
        }

        public override void ProcessItem(IQueueActivatorExecutionContext context)
        {
            ImportedCDRBatch cdrBatch = (ImportedCDRBatch)context.ItemToProcess;

            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            dataManager.SaveCDRsToDB(cdrBatch.CDRs);
        }
    }
}
