using System;

namespace Vanrise.Notification.Entities
{
    public interface IVRNotificationBeforeIdContext : IGetVRNotificationContext
    {
        long LessThanID { get; }

        Func<VRNotification, bool> onItemReady { get; }
    }

    public class VRNotificationBeforeIdContext : IVRNotificationBeforeIdContext
    {
        public Guid NotificationTypeId { get; set; }

        public long NbOfRows { get; set; }

        public long LessThanID { get; set; }

        public VRNotificationQuery Query { get; set; }

        public Func<VRNotification, bool> onItemReady { get; set; }
    }
}
