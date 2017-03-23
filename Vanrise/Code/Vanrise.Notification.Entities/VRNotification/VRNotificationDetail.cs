
namespace Vanrise.Notification.Entities
{
    public class VRNotificationDetail
    {
        public VRNotification Entity { get; set; }

        public string StatusDescription { get { if (this.Entity != null) return this.Entity.Status.ToString(); return null; } }
    }
}
