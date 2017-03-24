using System;

namespace Vanrise.Notification.Entities
{
    public interface IVRNotificationFirstPageContext : IGetVRNotificationContext
    {
        Func<VRNotification, bool> onItemReady { get; }

        byte[] MaxTimeStamp { set; }
    }

    public class VRNotificationFirstPageContext : IVRNotificationFirstPageContext
    {
        public Guid NotificationTypeId { get; set; }

        public long NbOfRows { get; set; }

        public VRNotificationQuery Query { get; set; }

        public Func<VRNotification, bool> onItemReady { get; set; }

        public byte[] MaxTimeStamp { get; set; }
    }
}
