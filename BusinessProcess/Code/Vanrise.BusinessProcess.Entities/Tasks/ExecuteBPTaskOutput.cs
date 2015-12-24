using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities.Tasks
{
    public enum ExecuteBPTaskResult { Succeeded, ProcessInstanceNotExists, TaskNotExists }

    public class ExecuteBPTaskOutput
    {
        public ExecuteBPTaskResult Result { get; set; }
    }
}
