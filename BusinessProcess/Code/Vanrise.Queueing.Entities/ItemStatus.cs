using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class ItemExecutionStatus
    {
        public long ItemId { get; set; }

        public long ExecutionFlowTriggerItemId { get; set; }

        public QueueItemStatus Status { get; set; }
    }
}
