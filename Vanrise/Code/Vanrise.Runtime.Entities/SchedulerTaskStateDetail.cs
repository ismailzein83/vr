using System;
using Vanrise.Common;

namespace Vanrise.Runtime.Entities
{
    public class SchedulerTaskStateDetail
    {
        public SchedulerTaskState Entity { get; set; }

        public bool IsEnabled { get; set; }

        public string StatusDescription { get { if (this.Entity != null) return Utilities.GetEnumDescription(this.Entity.Status); return null; } }

        public bool AllowRunIfEnabled { get; set; }
    }
}
