using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Queueing.Entities
{
    public enum HoldRequestStatus { Pending = 0, CanBeStarted = 1 }
    public class HoldRequest : IDateEffectiveSettings
    {
        public long HoldRequestId { get; set; }

        public long BPInstanceId { get; set; }

        public Guid ExecutionFlowDefinitionId { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public List<int> QueuesToHold { get; set; }

        public List<int> QueuesToProcess { get; set; }

        public HoldRequestStatus Status { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime BED { get { return From; } }

        public DateTime? EED { get { return To; } }
    }
}
