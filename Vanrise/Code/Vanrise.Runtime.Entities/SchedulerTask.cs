using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public enum SchedulerTaskStatus { NotStarted = 0, Started = 1, Stopped = 2 } 

    public class SchedulerTask
    {
        public int TaskId { get; set; }

        public string Name { get; set; }

        public bool IsEnabled { get; set; }

        public SchedulerTaskStatus Status { get; set; }

        public DateTime? NextRunTime { get; set; }

        public DateTime? LastRunTime { get; set; }

        public int TriggerTypeId { get; set; }

        public SchedulerTaskTrigger TaskTrigger { get; set; }

        public int ActionTypeId { get; set; }

        public SchedulerTaskAction TaskAction { get; set; }

    }
}
