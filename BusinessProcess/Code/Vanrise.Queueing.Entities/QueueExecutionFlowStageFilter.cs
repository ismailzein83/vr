using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class QueueExecutionFlowStageFilter
    {
        public  List<string> InculdesStageNames { set; get; }
        public List<IQueueExecutionFlowStageFilter> Filters { get; set; }
    }
}
