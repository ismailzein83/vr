using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class PendingQueueItemInfo
    {
        public long QueueItemId { get; set; }

        public int QueueId { get; set; }

        public Guid? ActivatorInstanceId { get; set; }
    }
}
