using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.RealTimeReport.Widgets
{
    public class RealTimeChartWidget:  RealTimeReportWidget
    {
        public List<RealTimeChartWidgetDimension> Dimensions { get; set; }
        public List<RealTimeChartWidgetMeasure> Measures { get; set; }
        public string TopMeasure { get; set; }
        public int TopRecords { get; set; }
        public string ChartType { get; set; }
        public override List<string> GetMeasureNames(){
            return this.Measures.Select(m => m.MeasureName).ToList();
        }
    }
   

    public class RealTimeChartWidgetDimension
    {
        public string DimensionName { get; set; }
        public string Title { get; set; }
    }

    public class RealTimeChartWidgetMeasure
    {
        public string MeasureName { get; set; }
        public string Title { get; set; }
    }
}
