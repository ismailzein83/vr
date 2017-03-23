using System;

namespace Vanrise.Notification.Entities
{
    public class VRNotificationBeforeIdInput : VRNotificationInput
    {
        public long LessThanID { get; set; }
    }
}