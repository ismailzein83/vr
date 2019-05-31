using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Analytic.MainExtensions.RealTimeReport.Widgets
{
    public class RealTimeChartWidget:  RealTimeReportWidget
    {
        public override Guid ConfigId { get { return  new Guid("DBEFFA6E-E75E-497F-8ACF-8F15469D9B90"); } }
        public List<RealTimeChartWidgetDimension> Dimensions { get; set; }
        public List<RealTimeChartWidgetMeasure> Measures { get; set; }
        public string TopMeasure { get; set; }
        public int TopRecords { get; set; }
        public string ChartType { get; set; }
        public override List<string> GetMeasureNames(){
            return this.Measures.Select(m => m.MeasureName).ToList();
        }
        public override void ApplyTranslation(IRealTimeReportWidgetTranslationContext context)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
            AnalyticItemConfigManager configManager = new AnalyticItemConfigManager();

            var dimensions = configManager.GetDimensions(context.AnalyticTableId);

            foreach (var dim in Dimensions)
            {
                var dimension = dimensions.GetRecord(dim.DimensionName);
                if (dimension != null && dimension.Config != null && dimension.Config.TitleResourceKey != null)
                    dim.Title = vrLocalizationManager.GetTranslatedTextResourceValue(dimension.Config.TitleResourceKey, dim.Title, context.LanguageId);
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
   

    public class RealTimeChartWidgetDimension
    {
        public string DimensionName { get; set; }
        public string Title { get; set; }
        public string TitleResourceKey { get; set; }

    }

    public class RealTimeChartWidgetMeasure
    {
        public string MeasureName { get; set; }
        public string Title { get; set; }
        public string TitleResourceKey { get; set; }

    }
}
