using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.BusinessProcess.Entities;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.Runtime.Entities;

namespace Vanrise.BusinessProcess.Web.Models
{
    public class SchedulerTaskModel
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

        public string StatusDescription { get; set; }
    }
}