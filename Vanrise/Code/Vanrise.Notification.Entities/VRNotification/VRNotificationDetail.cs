using Vanrise.Common;
using Vanrise.Entities;
using System.Linq;

namespace Vanrise.Notification.Entities
{
    public class VRNotificationDetail
    {
        public VRNotification Entity { get; set; }

        public VRNotificationDetailEventPayload VRNotificationEventPayload { get; set; }

        public string StatusDescription { get { return this.Entity != null ? Vanrise.Common.Utilities.GetEnumDescription<VRNotificationStatus>(this.Entity.Status) : null; } }

        public string AlertLevelDescription { get; set; }

        public StyleFormatingSettings AlertLevelStyle { get; set; }

        public string ActionNames
        {
            get
            {
                if (this.Entity == null || this.Entity.Data == null || this.Entity.Data.Actions == null || this.Entity.Data.Actions.Count == 0)
                    return string.Empty;

                return string.Join<string>(", ", this.Entity.Data.Actions.Select(x => x.ActionName));
            }
        }

        public string RollbackActionNames
        {
            get
            {
                if (this.Entity == null || this.Entity.Data == null || this.Entity.Data.ClearanceActions == null || this.Entity.Data.ClearanceActions.Count == 0)
                    return string.Empty;

                return string.Join<string>(", ", this.Entity.Data.ClearanceActions.Select(x => x.ActionName));
            }
        }
    }

    public abstract class VRNotificationDetailEventPayload
    {
    }
}