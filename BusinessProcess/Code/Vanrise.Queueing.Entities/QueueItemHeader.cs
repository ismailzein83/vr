using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public enum QueueItemStatus { New = 0, Processing = 10, Failed = 20, Processed = 30 }

    public class QueueItemHeader
    {
        public long ItemId { get; set; }

        public int QueueId { get; set; }

        public long SourceItemId { get; set; }

        public string Description { get; set; }

        public QueueItemStatus Status { get; set; }

        public int RetryCount { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime LastUpdatedTime { get; set; }
    }
}
