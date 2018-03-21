using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace Vanrise.Analytic.MainExtensions.History.Widgets
{
    public class AnalyticGridWidget : AnalyticHistoryReportWidget
    {
        public override Guid ConfigId { get { return  new Guid("7A2A35E2-543A-42C7-B97F-E05EE8D09A00"); } }
        public bool RootDimensionsFromSearchSection { get; set; }

        public List<AnalyticGridWidgetDimension> Dimensions { get; set; }

        public List<AnalyticGridWidgetMeasure> Measures { get; set; }

        public List<MeasureStyleRule> MeasureStyleRules { get; set; }
        public List<Entities.AnalyticItemAction> ItemActions { get; set; }
        public AnalyticQueryOrderType OrderType { get; set; }
        public Object AdvancedOrderOptions { get; set; }
        public bool WithSummary { get; set; }

        public override List<string> GetMeasureNames()
        {
            return this.Measures.Select(measure => measure.MeasureName).ToList();
        }
    }

    public class AnalyticGridWidgetDimension
    {
        public string DimensionName { get; set; }
        public string Title { get; set; }
        public bool IsRootDimension { get; set; }
        public GridColumnSettings ColumnSettings { get; set; }
        public Guid? ColumnStyleId { get; set; }
    }

    public class AnalyticGridWidgetMeasure
    {
        public string MeasureName { get; set; }

        public string Title { get; set; }
        public GridColumnSettings ColumnSettings { get; set; }
        public Guid? ColumnStyleId { get; set; }

    }
}
