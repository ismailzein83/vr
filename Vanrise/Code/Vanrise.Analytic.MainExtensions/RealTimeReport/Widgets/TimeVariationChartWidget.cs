using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.RealTimeReport.Widgets
{
    public class TimeVariationChartWidget : RealTimeReportWidget
    {
        public override Guid ConfigId { get { return  new Guid("CADC9403-6668-48E9-B452-D398B62921AB"); } }
        public List<RealTimeTimeVariationChartWidgetMeasure> Measures { get; set; }
        public string ChartType { get; set; }

        public override List<string> GetMeasureNames()
        {
            return this.Measures.Select(m => m.MeasureName).ToList();
        }
    }
    public class RealTimeTimeVariationChartWidgetMeasure
    {
        public string MeasureName { get; set; }
        public string Title { get; set; }
    }
}
