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
    public class TimeVariationChartWidget : RealTimeReportWidget
    {
        public override Guid ConfigId { get { return  new Guid("CADC9403-6668-48E9-B452-D398B62921AB"); } }
        public List<RealTimeTimeVariationChartWidgetMeasure> Measures { get; set; }
        public string ChartType { get; set; }

        public override List<string> GetMeasureNames()
        {
            return this.Measures.Select(m => m.MeasureName).ToList();
        }
        public override void ApplyTranslation(IRealTimeReportWidgetTranslationContext context)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
            AnalyticItemConfigManager configManager = new AnalyticItemConfigManager();

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
    public class RealTimeTimeVariationChartWidgetMeasure
    {
        public string MeasureName { get; set; }
        public string Title { get; set; }
        public string TitleResourceKey { get; set; }

    }
}
