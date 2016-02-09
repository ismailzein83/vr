using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class QueueExecutionFlowStage
    {
        public string StageName { get; set; }

        public string QueueNameTemplate { get; set; }

        public string QueueTitleTemplate { get; set; }
    }
}
