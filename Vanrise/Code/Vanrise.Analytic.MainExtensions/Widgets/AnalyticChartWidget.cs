using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.Widgets
{
    public class AnalyticChartWidget : AnalyticReportWidget
    {
        public List<AnalyticChartWidgetDimension> Dimensions { get; set; }
        public List<AnalyticChartWidgetMeasure> Measures { get; set; }
        public string TopMeasure { get; set; }
        public int TopRecords { get; set; }
        public string ChartType { get; set; }
    }

    public class AnalyticChartWidgetDimension
    {
        public string DimensionName { get; set; }
        public string Title { get; set; }
    }

    public class AnalyticChartWidgetMeasure
    {
        public string MeasureName { get; set; }
        public string Title { get; set; }
    }
}
