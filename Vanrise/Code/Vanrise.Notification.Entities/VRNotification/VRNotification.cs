﻿using System;
using System.Collections.Generic;

namespace Vanrise.Notification.Entities
{
    public enum VRNotificationStatus { New = 0, Executing = 10, Completed = 20, Cleared = 30, Suspended = 40 }

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
    }

    public class VRNotificationData
    {
        public IVRActionEventPayload EventPayload { get; set; }

        public List<VRAction> Actions { get; set; }

        public List<VRAction> ClearanceActions { get; set; }

        public bool IsAutoClearable { get; set; }
    }
}
