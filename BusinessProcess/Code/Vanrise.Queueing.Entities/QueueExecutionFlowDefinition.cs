using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class QueueExecutionFlowDefinition
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public List<QueueExecutionFlowStage> Stages { get; set; }
    }
}
