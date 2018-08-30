using System;

namespace Vanrise.Runtime.Entities
{
    public class SchedulerTaskDetail
    {
        public SchedulerTask Entity { get; set; }

        public bool HasEditPermission { get; set; }

        public bool HasRunPermission { get; set; }
        public bool AllowRunIfEnabled { get; set; }
    }
}
