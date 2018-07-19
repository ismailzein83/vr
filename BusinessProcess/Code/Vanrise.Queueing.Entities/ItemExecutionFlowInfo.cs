using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public enum ItemExecutionFlowStatus { New = 0, Processing = 10, Failed = 20, Processed = 30, Suspended = 40, NoBatches = 50 }

    public class ItemExecutionFlowInfo
    {
        public long ItemId { get; set; }

        public long ExecutionFlowTriggerItemId { get; set; }

        public ItemExecutionFlowStatus Status { get; set; }
    }
}
