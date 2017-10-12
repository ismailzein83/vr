using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class QueueItemHeaderDetails
    {
        public QueueItemHeader Entity { get; set; }

        public string StageName { get; set; }

        public string StatusName { get; set; }

        public string QueueTitle { get; set; }
 
        public string ExecutionFlowName { get; set; }

        public string DataSourceName { get; set; }
    }
}
