using System;

namespace Vanrise.Notification.Entities
{
    public interface IVRNotificationUpdateContext : IGetVRNotificationContext
    {
        object LastUpdateHandle { get; set; }
    }

    public class VRNotificationUpdateContext : IVRNotificationUpdateContext
    {
        public Guid NotificationTypeId { get; set; }

        public long NbOfRows { get; set; }

        public VRNotificationQuery Query { get; set; }

        public object LastUpdateHandle { get; set; }
    }
}