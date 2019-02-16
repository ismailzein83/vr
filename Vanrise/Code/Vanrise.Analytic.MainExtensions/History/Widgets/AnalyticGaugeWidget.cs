using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.History.Widgets
{
    public class AnalyticGaugeWidget : AnalyticHistoryReportWidget
    {
        public override Guid ConfigId { get { return new Guid("E9CC9A31-DD48-45F5-A849-5402CCE3B7AF"); } }
        public List<AnalyticChartWidgetMeasure> Measures { get; set; }
        public long Maximum { get; set; }
        public long Minimum { get; set; }
        public override List<string> GetMeasureNames()
        {
            return null;
                //this.Measures.Select(measure => measure.MeasureName).ToList();
        }
    }
}
