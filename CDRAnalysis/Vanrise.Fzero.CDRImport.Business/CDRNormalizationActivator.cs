using Vanrise.Fzero.CDRImport.Data;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.CDRImport.Business
{
    public class CDRNormalizationActivator : QueueActivator
    {
        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            ImportedCDRBatch cdrBatch = (ImportedCDRBatch)item;

            PSTN.BusinessEntity.Business.NormalizationRuleManager normalizationManager = new PSTN.BusinessEntity.Business.NormalizationRuleManager();
            foreach (var cdr in cdrBatch.CDRs)
            {
                normalizationManager.Normalize(cdr);
            }


            outputItems.Add("CDR Import", cdrBatch);
        }

        public override void OnDisposed()
        {
            
        }

    }
}
