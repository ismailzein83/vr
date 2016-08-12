using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcAlertRuleTypeSettings : VRAlertRuleTypeSettings
    {
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
                return "VR_Analytic_DAProfCalcAlertRule_VRAction";
            }
            set
            {
                base.VRActionExtensionType = value;
            }
        }

        public override string CheckerBPActivityFQTN
        {
            get
            {
                return "Vanrise.Analytic.BP.DAProfCalcAlertRuleSubProcess";
            }
            set
            {
                base.CheckerBPActivityFQTN = value;
            }
        }

        public Guid DataAnalysisDefinitionId { get; set; }

        public List<DAProfCalcAlertRuleRecordStorage> SourceRecordStorages { get; set; }
    }

    public class DAProfCalcAlertRuleRecordStorage
    {
        public int DataRecordStorageId { get; set; }
    }
}
