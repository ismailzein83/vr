using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.QueueActivators
{
    public class DataRecordBatchQueueItemType : Vanrise.Queueing.Entities.QueueExecutionFlowStageItemType
    {
        public Guid DataRecordTypeId { get; set; }

        public string BatchDescription { get; set; }

        public override Type GetQueueItemType()
        {
            return typeof(DataRecordBatch);
        }
    }
}
