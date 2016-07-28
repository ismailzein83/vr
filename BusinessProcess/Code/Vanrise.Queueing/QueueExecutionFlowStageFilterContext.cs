using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing
{
    public class QueueExecutionFlowStageFilterContext : IQueueExecutionFlowStageFilterContext
    {
        public QueueExecutionFlowStage Stage
        {
            get;
            set;
        }
    }
}
