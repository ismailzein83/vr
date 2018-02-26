using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Vanrise.Notification.Entities
{
    public enum VRNotificationStatus
    {
        [Description("New")]
        New = 0,
        [Description("Executing")]
        Executing = 10,
        [Description("Executed")]
        Executed = 20,
        [Description("Rolled Back")]
        RolledBack = 30,
        [Description("Execution Error")]
        ExecutionError = 40,
        [Description("Rollback Error")]
        RollbackError = 50,
        [Description("Rollback")]
        Rollback = 60
    }

    public class VRNotification
    {
        public long VRNotificationId { get; set; }

        public int UserId { get; set; }

        public Guid TypeId { get; set; }

        public VRNotificationParentTypes ParentTypes { get; set; }

        public string EventKey { get; set; }

        public long? ExecuteBPInstanceID { get; set; }

        public long? ClearBPInstanceID { get; set; }

        public VRNotificationStatus Status { get; set; }

        public string Description { get; set; }

        public Guid AlertLevelId { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime CreationTime { get; set; }

        public VRNotificationData Data { get; set; }

        public IVRActionEventPayload EventPayload { get; set; }

        public IVRActionRollbackEventPayload RollbackEventPayload { get; set; }
    }

    public enum VRNotificationBPInstanceType { Execute, Clear }

    public class VRNotificationData
    {
        public List<VRAction> Actions { get; set; }

        public List<VRAction> ClearanceActions { get; set; }

        public bool IsAutoClearable { get; set; }
    }
}