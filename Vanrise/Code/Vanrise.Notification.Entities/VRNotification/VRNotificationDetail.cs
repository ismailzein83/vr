using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Notification.Entities
{
    public class VRNotificationDetail
    {
        public VRNotification Entity { get; set; }

        public VRNotificationDetailEventPayload VRNotificationEventPayload { get; set; }

        public string StatusDescription { get { return this.Entity != null ? Vanrise.Common.Utilities.GetEnumDescription<VRNotificationStatus>(this.Entity.Status) : null; } }

        public string AlertLevelDescription { get; set; }

        public StyleFormatingSettings AlertLevelStyle { get; set; }
    }

    public abstract class VRNotificationDetailEventPayload
    {
    }
}