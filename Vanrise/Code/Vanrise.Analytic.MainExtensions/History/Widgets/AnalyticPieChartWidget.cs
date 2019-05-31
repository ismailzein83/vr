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
    public class AnalyticPieChartWidget : AnalyticHistoryReportWidget
    {
        public override Guid ConfigId { get { return new Guid("CD2AFBD8-5C2F-4E50-8F36-F57C35A0C10F"); } }
        public List<AnalyticChartWidgetDimension> Dimensions { get; set; }
        public AnalyticChartWidgetMeasure Measure { get; set; }
        public int TopRecords { get; set; }

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

            if (Measure != null)
            {
                var measures = configManager.GetMeasures(context.AnalyticTableId);
             
                    var measure = measures.GetRecord(Measure.MeasureName);
                    if (measure != null && measure.Config != null && measure.Config.TitleResourceKey != null)
                    Measure.Title = vrLocalizationManager.GetTranslatedTextResourceValue(measure.Config.TitleResourceKey, Measure.Title, context.LanguageId);
            }
        }
        public override List<string> GetMeasureNames()
        {
            return new List<string> { this.Measure.MeasureName };
        }
    }

}
