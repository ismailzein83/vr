using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class AnalyticHistoryReportWidget
    {
        public int ConfigId { get; set; }

        public int AnalyticTableId { get; set; }
        public string WidgetTitle { get; set; }
        public int ColumnWidth { get; set; }
        public bool ShowTitle { get; set; }

        public abstract List<string> GetMeasureNames();
    }
}
