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
    public class AnalyticGaugeWidget : AnalyticHistoryReportWidget
    {
        public override Guid ConfigId { get { return new Guid("E9CC9A31-DD48-45F5-A849-5402CCE3B7AF"); } }
        public List<AnalyticChartWidgetMeasure> Measures { get; set; }
        public long Maximum { get; set; }
        public long Minimum { get; set; }
        public bool AutoRefresh { get; set; }
        public int? AutoRefreshInterval { get; set; }
        public override List<string> GetMeasureNames()
        {
            return null;
                //this.Measures.Select(measure => measure.MeasureName).ToList();
        }
        public override void ApplyTranslation(IAnalyticHistoryReportWidgetTranslationContext context)
        {
            AnalyticItemConfigManager configManager = new AnalyticItemConfigManager();
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();

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
}
 