using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class CreateAlertRuleNotificationInput
    {
        public int UserId { get; set; }

        public string Description { get; set; }

        public Guid AlertLevelId { get; set; }

        public Guid AlertRuleTypeId { get; set; }

        public Guid NotificationTypeId { get; set; }

        public long AlertRuleId { get; set; }

        public string EventKey { get; set; }

        public IVRActionEventPayload EventPayload { get; set; }

        public List<VRAction> Actions { get; set; }

        public List<VRAction> ClearanceActions { get; set; }

        public bool IsAutoClearable { get; set; }

        public string EntityId { get; set; }
    }
}
