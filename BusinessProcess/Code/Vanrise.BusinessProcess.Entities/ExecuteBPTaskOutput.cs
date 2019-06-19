using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public enum ExecuteBPTaskResult { Succeeded = 0, Failed = 1 }

    public class ExecuteBPTaskOutput
    {
            public ExecuteBPTaskResult Result { get; set; }
            public string OutputMessage { get; set; }
    }
}
