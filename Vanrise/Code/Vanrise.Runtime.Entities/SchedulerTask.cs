﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class SchedulerTask
    {
        public int TaskId { get; set; }

        public string Name { get; set; }

        public bool IsEnabled { get; set; }

        public int TriggerTypeId { get; set; }

        public SchedulerTaskTrigger TaskTrigger { get; set; }

        public int ActionTypeId { get; set; }

        public SchedulerTaskAction TaskAction { get; set; }

    }
}
