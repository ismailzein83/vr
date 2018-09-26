using System;

namespace Vanrise.Notification.Entities
{
    public class VRAlertRule
    {
        public long VRAlertRuleId { get; set; }

        public string Name { get; set; }

        public Guid RuleTypeId { get; set; }

        public int UserId { get; set; }

        public VRAlertRuleSettings Settings { get; set; }

        public bool IsDisabled { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedTime { get; set; }
    }
}