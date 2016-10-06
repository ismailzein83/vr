using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.QueueActivators
{
    public class UpdateSummaryQueueActivator : Vanrise.Queueing.Entities.QueueActivator, Vanrise.Queueing.Entities.ISummaryBatchQueueActivator
    {
        public Guid SummaryTransformationDefinitionId { get; set; }

        GenericSummaryTransformationManager _summaryTransformationManager;
        GenericSummaryTransformationManager SummaryTransformationManager
        {
            get
            {
                if (_summaryTransformationManager == null)
                {
                    _summaryTransformationManager = new GenericSummaryTransformationManager
                        {
                            SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId
                        };
                }
                return _summaryTransformationManager;
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
