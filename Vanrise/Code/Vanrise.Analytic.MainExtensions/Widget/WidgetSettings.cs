using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Analytic.MainExtensions.Widget
{
    public class WidgetSettings : VRTileExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("76285A3E-C385-49C3-8833-DEA21A40471E"); } }

        public override string RuntimeEditor { get { return "vr-common-widgetsettings-runtime"; } }
        public Guid AnalyticTableId { get; set; }
        public VRTimePeriod TimePeriod { get; set; }
        public RecordFilter RecordFilter { get; set; }
        public AnalyticHistoryReportWidget Settings { get; set; }
    }
}
