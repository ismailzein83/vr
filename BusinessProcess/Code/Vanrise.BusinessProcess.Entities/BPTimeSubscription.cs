using System;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPTimeSubscription
    {
        public long BPTimeSubscriptionId { get; set; }

        public long ProcessInstanceId { get; set; }

        public string Bookmark { get; set; }

        public BPTimeSubscriptionPayload Payload { get; set; }

        public DateTime DueTime { get; set; }
    }

    public abstract class BPTimeSubscriptionPayload 
    {

    }
}