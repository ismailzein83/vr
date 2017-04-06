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
        [Description("Cleared")]
        Cleared = 30,
        [Description("Error")]
        ExecutionError = 40,
        [Description("Error")]
        ClearanceError = 50
    }

    public class VRNotification
    {
        public long VRNotificationId { get; set; }

        public int UserId { get; set; }

        public Guid TypeId { get; set; }

        public VRNotificationParentTypes ParentTypes { get; set; }

        public string EventKey { get; set; }

        public long? BPProcessInstanceId { get; set; }

        public VRNotificationStatus Status { get; set; }

        public string Description { get; set; }

        public Guid AlertLevelId { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime CreationTime { get; set; }

        public VRNotificationData Data { get; set; }

        public IVRActionEventPayload EventPayload { get; set; }
    }

    public class VRNotificationData
    {
        public List<VRAction> Actions { get; set; }

        public List<VRAction> ClearanceActions { get; set; }

        public bool IsAutoClearable { get; set; }
    }
}