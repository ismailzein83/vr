using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.GenericData.QueueActivators
{
    public class RecordTypeStageFilter : IQueueExecutionFlowStageFilter
    {
        public Guid RecordTypeId { get; set; }

        public bool IsMatch(IQueueExecutionFlowStageFilterContext context)
        {
            DataRecordBatchQueueItemType dataRecordItemType = context.Stage.QueueItemType as DataRecordBatchQueueItemType;
            if (dataRecordItemType == null)
                return false;
            return dataRecordItemType.DataRecordTypeId == this.RecordTypeId;
        }
    }
}
