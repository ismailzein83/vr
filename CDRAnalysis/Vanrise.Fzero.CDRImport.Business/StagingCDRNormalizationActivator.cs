using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.CDRImport.Business
{
    public class StagingCDRNormalizationActivator : QueueActivator
    {
       
        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
            ImportedStagingCDRBatch cdrBatch = (ImportedStagingCDRBatch)item;

            PSTN.BusinessEntity.Business.NormalizationRuleManager normalizationManager = new PSTN.BusinessEntity.Business.NormalizationRuleManager();
            foreach(var cdr in cdrBatch.StagingCDRs)
            {
                normalizationManager.Normalize(cdr);
            }

            outputItems.Add("Save CDRs", cdrBatch);
        }

        public override void OnDisposed()
        {

        }

    }
}
