using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class QueueItemStatusSummary
    {

        public int QueueId { get; set; }

        public QueueItemStatus Status { get; set; }

        public int Count { get; set; }

    }
}
