using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public enum QueueItemStatus {
       [Description("New")]
        New = 0,
       [Description("Processing")]
       Processing = 10,
       [Description("Failed")]
       Failed = 20,
       [Description("Processed")]
       Processed = 30,
       [Description("Suspended")]
       Suspended = 40
    }

    public class QueueItemHeader
    {
        public long ItemId { get; set; }

        public int QueueId { get; set; }

        public long ExecutionFlowTriggerItemId { get; set; }

        public Guid? DataSourceID { get; set; }

        public string BatchDescription { get; set; }

        public long SourceItemId { get; set; }

        public string Description { get; set; }

        public QueueItemStatus Status { get; set; }

        public int RetryCount { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime LastUpdatedTime { get; set; }
    }
}
