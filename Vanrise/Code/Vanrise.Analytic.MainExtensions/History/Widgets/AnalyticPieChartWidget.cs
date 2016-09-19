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
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("CD2AFBD8-5C2F-4E50-8F36-F57C35A0C10F"); } }
        public List<AnalyticChartWidgetDimension> Dimensions { get; set; }
        public AnalyticChartWidgetMeasure Measure { get; set; }
        public int TopRecords { get; set; }


        public override List<string> GetMeasureNames()
        {
            return new List<string> { this.Measure.MeasureName };
        }
    }

}
