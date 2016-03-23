using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.QueueActivators
{
    public class UpdateSummaryQueueActivator : Vanrise.Queueing.Entities.QueueActivator, Vanrise.Queueing.Entities.ISummaryBatchQueueActivator
    {
        public int SummaryTransformationDefinitionId { get; set; }
        GenericSummaryTransformationManager SummaryTransformationManager
        {
            get
            {
                return new GenericSummaryTransformationManager
                {
                    SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId
                }; 
            }
        }

        public bool TryLock(DateTime batchStart)
        {
            return this.SummaryTransformationManager.TryLock(batchStart);
        }

        public void Unlock(DateTime batchStart)
        {
            this.SummaryTransformationManager.Unlock(batchStart);
        }

        public void UpdateNewBatches(DateTime batchStart, IEnumerable<Queueing.Entities.PersistentQueueItem> newBatches)
        {
            this.SummaryTransformationManager.UpdateNewBatches(batchStart, newBatches.Select(itm => itm as GenericSummaryRecordBatch));
        }
    }
}
