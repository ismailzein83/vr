using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class CreateVRNotificationInput
    {
        public int UserId { get; set; }

        public Guid NotificationTypeId { get; set; }

        public VRNotificationParentTypes ParentTypes { get; set; }

        public string EventKey { get; set; }

        public string Description { get; set; }

        public Guid AlertLevelId { get; set; }

        public IVRActionEventPayload EventPayload { get; set; }

        public List<VRAction> Actions { get; set; }

        public List<VRAction> ClearanceActions { get; set; }

        public bool IsAutoClearable { get; set; }

        public string EntityId { get; set; }
    }

    public class VRNotificationParentTypes
    {
        public string ParentType1 { get; set; }

        public string ParentType2 { get; set; }
    }
}
