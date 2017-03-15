﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public override Guid ConfigId { get { return new Guid("57033e80-65cb-4359-95f6-22a57084d027"); } }

        public override Guid NotificationTypeId { get { return new Guid("6BB06963-AC64-4827-A7FC-EB6892057AD7"); } }

        public Guid DataAnalysisDefinitionId { get; set; }

        public List<DAProfCalcAlertRuleRecordStorage> SourceRecordStorages { get; set; }

        public override string SettingEditor
        {
            get
            {
                return "vr-analytic-daprofcalc-alertrulesettings";
            }
            set
            {
                base.SettingEditor = value;
            }
        }
    }

    public class DAProfCalcAlertRuleRecordStorage
    {
        public Guid DataRecordStorageId { get; set; }
    }
}
