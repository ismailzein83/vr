using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.CDRImport.Data;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.CDRImport.Business
{
    public class CDRImportActivator : QueueActivator
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
