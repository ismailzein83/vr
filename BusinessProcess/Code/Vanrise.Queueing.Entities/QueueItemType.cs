using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class QueueItemType
    {
        public long Id { get; set; }

        public string ItemFQTN { get; set; }

        public string Title { get; set; }

        public string DefaultQueueSettings { get; set; }

        public DateTime CreatedTime { get; set; }

    }
}
