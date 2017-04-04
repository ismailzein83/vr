using System;

namespace Vanrise.Notification.Entities
{
    public abstract class VRNotificationTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual string SearchRuntimeEditor { get; set; }

        public virtual string BodyRuntimeEditor { get; set; }

        public abstract bool IsVRNotificationMatched(IVRNotificationTypeIsMatchedContext context);

        public abstract VRNotificationDetailEventPayload GetNotificationDetailEventPayload(IVRNotificationTypeGetNotificationEventPayloadContext context);
    }
}