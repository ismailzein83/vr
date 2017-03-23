
namespace Vanrise.Notification.Entities
{
    public interface IVRNotificationTypeIsMatchedContext
    {
        VRNotification VRNotification { get; }

        VRNotificationExtendedQuery ExtendedQuery { get; }
    }

    public class VRNotificationTypeIsMatchedContext : IVRNotificationTypeIsMatchedContext
    {
        public VRNotification VRNotification { get; set; }

        public VRNotificationExtendedQuery ExtendedQuery { get; set; }
    }
}
