using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordActionRuleTypeSettings : VRAlertRuleTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("434F7B1E-8B93-4144-9150-E24BF3EB4EFB"); } }

        public Guid DataRecordTypeId { get; set; }

        public List<ActionRuleTypeRecordField> IdentificationFields { get; set; }

        public override Guid NotificationTypeId { get; set; }
    }

    public class ActionRuleTypeRecordField
    {
        public string Name { get; set; }
    }
}
