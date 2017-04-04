
namespace Vanrise.Notification.Entities
{
    public interface IVRNotificationTypeGetNotificationEventPayloadContext
    {
        VRNotification VRNotification { get; }
    }

    public class VRNotificationTypeGetNotificationEventPayloadContext : IVRNotificationTypeGetNotificationEventPayloadContext
    {
        public VRNotification VRNotification { get; set; }
    }
}
