﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{

    public enum SchedulerTaskStatus
    {
        [Description("Not Started")] NotStarted = 0,

        [Description("Running")] InProgress = 1,

        [Description("Completed")] Completed = 2,

        [Description("Failed")] Failed = 3,
        [Description("Running")] WaitingEvent = 4
    }

    public enum SchedulerTaskType
    {
        System = 0,
        User = 1
    } 

    public class SchedulerTask
    {
        public int TaskId { get; set; }

        public string Name { get; set; }

        public bool IsEnabled { get; set; }

        public SchedulerTaskType TaskType { get; set; }

        public SchedulerTaskStatus Status { get; set; }

        public DateTime? NextRunTime { get; set; }

        public DateTime? LastRunTime { get; set; }

        public int TriggerTypeId { get; set; }

        public int ActionTypeId { get; set; }

        public TriggerTypeInfo TriggerInfo { get; set; }

        public ActionTypeInfo ActionInfo { get; set; }
        public int OwnerId { get; set; }
        public Object ExecutionInfo { get; set; }

        public SchedulerTaskSettings TaskSettings { get; set; }


        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

    }

    public class SchedulerTaskSettings
    {
        public BaseTaskActionArgument TaskActionArgument { get; set; }

        public BaseTaskTriggerArgument TaskTriggerArgument { get; set; }

        public DateTime StartEffDate { get; set; }

        public DateTime? EndEffDate { get; set; }
    }
}
