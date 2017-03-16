
namespace Vanrise.Notification.Entities
{
    public interface IVRNotificationTypeFilter
    {
        bool IsMatched(IVRNotificationTypeFilterContext context);
    }

    public interface IVRNotificationTypeFilterContext
    {
        VRNotificationType VRNotificationType { get; }
    }

    public class VRNotificationTypeFilterContext : IVRNotificationTypeFilterContext
    {
        public VRNotificationType VRNotificationType { get; set; }
    }
}