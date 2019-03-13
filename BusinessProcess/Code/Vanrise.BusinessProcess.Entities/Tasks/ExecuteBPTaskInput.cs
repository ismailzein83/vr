using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class ExecuteBPTaskInput
    {
        public long TaskId { get; set; }

        public BPTaskExecutionInformation ExecutionInformation { get; set; }

        public int ExecutedBy { get; set; }

        public string Notes { get; set; }

        public string Decision { get; set; }
        public BPTaskData TaskData { get; set; }
    }
}
