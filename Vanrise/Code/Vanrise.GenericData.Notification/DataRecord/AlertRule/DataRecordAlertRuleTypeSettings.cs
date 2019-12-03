using System;
using System.Collections.Generic;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordAlertRuleTypeSettings : VRAlertRuleTypeSettings
    {
        public static Guid s_configID = new Guid("434F7B1E-8B93-4144-9150-E24BF3EB4EFB");
        public override Guid ConfigId { get { return s_configID; } }

        public override string SettingEditor { get { return "vr-genericdata-datarecordalertrule-extendedsettings"; } }

        public Guid DataRecordTypeId { get; set; }

        public Guid NotificationTypeId { get; set; }

        public DRAlertRuleTypeSettingsAdvancedFilters AdvancedFilters { get; set; }

        public List<AlertRuleTypeRecordField> IdentificationFields { get; set; }
    }

    public class AlertRuleTypeRecordField
    {
        public string Name { get; set; }

        public bool IsRequired { get; set; }

        public bool IsSelected { get; set; }
    }

    public class DRAlertRuleTypeSettingsAdvancedFilters
    {
        public AdvancedFilterFieldsRelationType FieldsRelationType { get; set; }
        public List<AdvancedFilterField> AvailableFields { get; set; }
    }

    public enum AdvancedFilterFieldsRelationType { AllFields = 0, SpecificFields = 1 }
    public class AdvancedFilterField
    {
        public string FieldName { get; set; }
    }
}