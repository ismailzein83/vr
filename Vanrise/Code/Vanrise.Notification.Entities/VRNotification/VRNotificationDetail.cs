using Vanrise.Common;

namespace Vanrise.Notification.Entities
{
    public class VRNotificationDetail
    {
        public VRNotification Entity { get; set; }

        public string StatusDescription { get { return this.Entity != null ? Vanrise.Common.Utilities.GetEnumDescription<VRNotificationStatus>(this.Entity.Status) : null; } }
    }
}