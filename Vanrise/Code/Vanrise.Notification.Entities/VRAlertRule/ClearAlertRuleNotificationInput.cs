using System;

namespace Vanrise.Notification.Entities
{
    public class ClearAlertRuleNotificationInput
    {
        public int UserId { get; set; }

        public Guid RuleTypeId { get; set; }

        public long? AlertRuleId { get; set; }

        public string EventKey { get; set; }

        public IVRActionRollbackEventPayload RollbackEventPayload { get; set; }

        public string Description { get; set; }

        public Guid NotificationTypeId { get; set; }

        public string EntityId { get; set; }
    }
}