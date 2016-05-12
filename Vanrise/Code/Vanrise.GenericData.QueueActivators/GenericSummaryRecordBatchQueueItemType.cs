using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.QueueActivators
{
    public class GenericSummaryRecordBatchQueueItemType : DataRecordBatchQueueItemType
    {
        public override Type GetQueueItemType()
        {
            return typeof(GenericSummaryRecordBatch);
        }
    }
}
