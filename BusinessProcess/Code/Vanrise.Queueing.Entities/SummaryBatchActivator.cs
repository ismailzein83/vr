using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class SummaryBatchActivator
    {
        public int QueueId { get; set; }

        public DateTime BatchStart { get; set; }

        public Guid ActivatorId { get; set; }
    }
}
