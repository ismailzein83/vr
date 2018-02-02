using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class SubscriberPreviewObject
    {
        public IEnumerable<SubscriberPreviewDetail> SubscriberPreviewDetails { get; set; }
        public SubscriberPreviewSummary SubscriberPreviewSummary { get; set; }
    }

    public class SubscriberPreview
    {
        public int SubscriberId { get; set; }
        public SubscriberProcessStatus Status { get; set; }
        public string Description { get; set; }
    }

    public class SubscriberPreviewDetail
    {
        public SubscriberPreview Entity { get; set; }
        public string SubscriberName { get; set; }
    }

    public class SubscriberPreviewSummary
    {
        public int NumberOfSubscriberWithSuccessStatus { get; set; }
        public int NumberOfSubscriberWithNoChangeStatus { get; set; }
        public int NumberOfSubscriberWithFailedStatus { get; set; }
    }

    public enum SubscriberProcessStatus
    {
        Success = 0,
        NoChange = 1,
        Failed = 2
    }
}
