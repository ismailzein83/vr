using System;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Business
{
    public class VRActionExecutionContext : IVRActionExecutionContext
    {
        public long? BPProcessInstanceId { get; set; }

        public IVRActionEventPayload EventPayload { get; set; }

        public IVRActionRollbackEventPayload RollbackEventPayload { get; set; }

        public int NumberOfExecutions { get; set; }
        public int UserID { get; set; }
        public DateTime? NextExecutionTime { get; set; }
        public long? AlertRuleId { get; set; }
    }

    public class VRActionConvertToBPInputArgumentContext : IVRActionConvertToBPInputArgumentContext
    {
        public IVRActionEventPayload EventPayload { get; set; }

        public IVRActionRollbackEventPayload RollbackEventPayload { get; set; }

        public int NumberOfExecutions { get; set; }

        public DateTime? NextExecutionTime { get; set; }

        public Vanrise.BusinessProcess.Entities.BaseProcessInputArgument BPInputArgument { get; set; }
    }
}
