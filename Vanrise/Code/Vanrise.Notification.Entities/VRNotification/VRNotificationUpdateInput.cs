using System;

namespace Vanrise.Notification.Entities
{
    public class VRNotificationUpdateInput : VRNotificationInput
    {
        public byte[] LastUpdateHandle { get; set; }
    }
}