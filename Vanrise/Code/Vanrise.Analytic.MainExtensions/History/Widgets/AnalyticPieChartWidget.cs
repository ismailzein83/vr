using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.History.Widgets
{
    public class AnalyticPieChartWidget : AnalyticHistoryReportWidget
    {
        public List<AnalyticChartWidgetDimension> Dimensions { get; set; }
        public AnalyticChartWidgetMeasure Measure { get; set; }
        public int TopRecords { get; set; }


        public override List<string> GetMeasureNames()
        {
            return new List<string> { this.Measure.MeasureName };
        }
    }

}
