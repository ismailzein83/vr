using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.Queries
{
    public class AnalyticTableQueryDefinitionSettings : VRAutomatedReportQueryDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("4ECC5DC2-5781-437A-AF6C-ACAEDC3C4A5D"); }
        }

        public Guid AnalyticTableId { get; set; }

        public override string RuntimeEditor { get { return "vr-analytic-analytictablequerydefinitionsettings-runtimeeditor"; } }

        public override bool DoesUserHaveAccess(IVRAutomatedReportQueryDefinitionExtendedSettingsContext context)
        {
            AnalyticTableManager analyticTableManager = new AnalyticTableManager();
            return analyticTableManager.DoesUserHaveAccess(context.LoggedInUserId, AnalyticTableId);
        }
    }
}
