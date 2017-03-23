
namespace Vanrise.Notification.Entities
{
    public interface IVRNotificationTypeMapToDetailContext
    {
        VRNotification VRNotification { get; }
    }

    public class VRNotificationTypeMapToDetailContext : IVRNotificationTypeMapToDetailContext
    {
        public VRNotification VRNotification { get; set; }
    }
}
