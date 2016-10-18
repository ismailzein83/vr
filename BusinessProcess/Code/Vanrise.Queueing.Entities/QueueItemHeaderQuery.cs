using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
   public class QueueItemHeaderQuery
    {

       public List<Guid> ExecutionFlowIds { get; set; }

        public List<int> QueueIds { get; set; }

        public List<QueueItemStatus> QueueStatusIds { get; set; }

        public DateTime? CreatedTimeFrom { get; set; }

        public DateTime? CreatedTimeTo { get; set; }

    }
}
