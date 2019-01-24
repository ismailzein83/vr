using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Business
{
    public class AnalyticWidgetTile : VRTileExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("D3A56F2D-34D5-4DCD-98B5-BA9564303645"); } }

        public override string RuntimeEditor { get { return ""; } }
        public AnalyticHistoryReportWidget WidgetSettings { get; set; }
        public RecordFilter RecordFilter { get; set; }
        public VRTimePeriod TimePeriod { get; set; }
    }
}
