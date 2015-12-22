using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    public class SchedulerTaskCheckProgressContext : ISchedulerTaskCheckProgressContext
    {
        public object ExecutionInfo { get; set; }
    }
}
