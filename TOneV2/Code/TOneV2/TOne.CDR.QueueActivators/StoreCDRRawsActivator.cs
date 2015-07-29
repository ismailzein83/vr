using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Object preparedMainCDRs = dataManager.PrepareCDRsForDBApply(cdrBatch.CDRs, cdrBatch.SwitchId);
            dataManager.ApplyCDRsToDB(preparedMainCDRs);
        }
        public override void OnDisposed()
        {
            
        }
    }
}
