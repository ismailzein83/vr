using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public enum ExecuteOutputResult { Completed, WaitingEvent }
    public class SchedulerTaskExecuteOutput
    {
        public ExecuteOutputResult Result { get; set; }

        public Object ExecutionInfo { get; set; }
    }
}
