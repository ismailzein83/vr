using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Queueing.Entities
{
    public class PendingQueueItemInfo : IDateEffectiveSettings
    {
        public long QueueItemId { get; set; }

        public long ExecutionFlowTriggerItemID { get; set; }

        public int QueueId { get; set; }

        public Guid? ActivatorInstanceId { get; set; }

        public DateTime BatchStart { get; set; }

        public DateTime BatchEnd { get; set; }

        public DateTime BED { get { return BatchStart; } }

        public DateTime? EED { get { return BatchEnd; } }
    }
}
