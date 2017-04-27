using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Queueing.Entities
{
    public class SummaryBatchActivator : IDateEffectiveSettings
    {
        public int QueueId { get; set; }

        public DateTime BatchStart { get; set; }

        public DateTime BatchEnd { get; set; }

        public Guid ActivatorId { get; set; }

        public DateTime BED { get { return BatchStart; } }

        public DateTime? EED { get { return BatchEnd; } }
    }
}
