using System;

namespace Vanrise.Notification.Entities
{
    public class VRNotificationUpdateInput : VRNotificationInput
    {
        public object LastUpdateHandle { get; set; }
    }
}