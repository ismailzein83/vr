using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordAlertRuleTypeSettings : VRAlertRuleTypeSettings
    {
        public static Guid s_configID = new Guid("434F7B1E-8B93-4144-9150-E24BF3EB4EFB");
        public override Guid ConfigId { get { return s_configID; } }

        public override string SettingEditor { get { return "vr-genericdata-datarecordalertrule-extendedsettings"; } }

        public Guid DataRecordTypeId { get; set; }

        public List<AlertRuleTypeRecordField> IdentificationFields { get; set; }

        public override Guid NotificationTypeId { get; set; }
    }

    public class AlertRuleTypeRecordField
    {
        public string Name { get; set; }
    }
}
