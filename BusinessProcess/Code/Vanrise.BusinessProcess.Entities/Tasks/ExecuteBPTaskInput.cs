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

        public int ExecutedBy { get; set; }

        public BPTaskAction TakenAction { get; set; }

        public BPTaskComment Comment { get; set; }
    }
}
