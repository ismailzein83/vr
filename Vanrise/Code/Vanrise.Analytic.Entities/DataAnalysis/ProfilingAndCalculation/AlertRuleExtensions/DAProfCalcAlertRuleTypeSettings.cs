using System;
using System.Collections.Generic;
using Vanrise.Notification.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcAlertRuleTypeSettings : VRAlertRuleTypeSettings
    {
        #region TO REMOVE

        public override string CriteriaEditor
        {
            get
            {
                return "vr-analytic-daprofcalc-alertrulecriteria";
            }
            set
            {
                base.CriteriaEditor = value;
            }
        }

        public override string VRActionExtensionType
        {
            get
            {
                return "Analytic_DAProfCalc_AlertRuleAction";
            }
            set
            {
                base.VRActionExtensionType = value;
            }
        }

        #endregion

        public static Guid s_ConfigId = new Guid("57033e80-65cb-4359-95f6-22a57084d027");
        public override Guid ConfigId { get { return s_ConfigId; } }

        public override Guid NotificationTypeId { get { return new Guid("6BB06963-AC64-4827-A7FC-EB6892057AD7"); } }

        public override string SettingEditor { get { return "vr-analytic-daprofcalc-alertrulesettings"; } }

        public Guid DataAnalysisDefinitionId { get; set; }

        public List<DAProfCalcAlertRuleRecordStorage> SourceRecordStorages { get; set; }

        public List<DAProfCalcItemNotification> DAProfCalcItemNotifications { get; set; }

        public string RawRecordFilterLabel { get; set; }
    }

    public class DAProfCalcAlertRuleRecordStorage
    {
        public Guid DataRecordStorageId { get; set; }
    }

    public class DAProfCalcItemNotification
    {
        public Guid DataAnalysisItemDefinitionId { get; set; }

        public Guid NotificationTypeId { get; set; }
    }
}