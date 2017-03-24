using System;

namespace Vanrise.Notification.Entities
{
    public interface IGetVRNotificationContext
    {
        Guid NotificationTypeId { get; }

        long NbOfRows { get; }

        VRNotificationQuery Query { get; }
    }
}
