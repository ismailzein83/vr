using System;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Web.Models
{
    public class QueueItemHeaderModel
    {
        public long ItemId { get; set; }

        public int QueueId { get; set; }

        public long SourceItemId { get; set; }

        public string Description { get; set; }

        public QueueItemStatus Status { get; set; }

        public String StatusDescription { get; set; }

        public int RetryCount { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime LastUpdatedTime { get; set; }
    }
}