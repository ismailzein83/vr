using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Analytic.MainExtensions.History.Widgets
{
    public enum AutoRefreshType { SummaryValues = 1 }
    public enum TimeOnXAxis { DateTime = 0, Time = 1 }
    public class AnalyticChartWidget : AnalyticHistoryReportWidget
    {
        public override Guid ConfigId { get { return new Guid("D050DEB3-700E-437B-86D1-510A81C0C14C"); } }
        public List<AnalyticChartWidgetDimension> Dimensions { get; set; }
        public List<AnalyticChartWidgetMeasure> Measures { get; set; }
        public List<AnalyticChartWidgetDimension> SeriesDimensions { get; set; }
        public AnalyticQueryOrderType OrderType { get; set; }
        public Object AdvancedOrderOptions { get; set; }
        public int? TopRecords { get; set; }
        public string ChartType { get; set; }
        public bool RootDimensionsFromSearch { get; set; }
        public int? AutoRefreshInterval { get; set; }
        public AutoRefreshType? AutoRefreshType { get; set; }
        public TimeOnXAxis? TimeOnXAxis { get; set; }
        public int? NumberOfPoints { get; set; }
        public List<Entities.AnalyticItemAction> ItemActions { get; set; }
        public override List<string> GetMeasureNames()
        {
            return this.Measures.Select(measure => measure.MeasureName).ToList();
        }
        public override void ApplyTranslation(IAnalyticHistoryReportWidgetTranslationContext context)
        {
            AnalyticItemConfigManager configManager = new AnalyticItemConfigManager();
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();

            if (Dimensions != null && Dimensions.Count > 0)
            {
                var dimensions = configManager.GetDimensions(context.AnalyticTableId);

                foreach (var widgetDimension in Dimensions)
                {
                    var dimension = dimensions.GetRecord(widgetDimension.DimensionName);
                    if (dimension != null && dimension.Config != null && dimension.Config.TitleResourceKey != null)
                        widgetDimension.Title = vrLocalizationManager.GetTranslatedTextResourceValue(dimension.Config.TitleResourceKey, widgetDimension.Title, context.LanguageId);
                }
            }

            if (Measures != null && Measures.Count > 0)
            {
                var measures = configManager.GetMeasures(context.AnalyticTableId);

                foreach (var widgetMeasure in Measures)
                {
                    var measure = measures.GetRecord(widgetMeasure.MeasureName);
                    if (measure != null && measure.Config != null && measure.Config.TitleResourceKey != null)
                        widgetMeasure.Title = vrLocalizationManager.GetTranslatedTextResourceValue(measure.Config.TitleResourceKey, widgetMeasure.Title, context.LanguageId);
                }
            }
        }
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
