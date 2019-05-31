using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public abstract class RealTimeReportWidget
    {
        public abstract Guid ConfigId { get; }
        public Guid AnalyticTableId { get; set; }
        public string WidgetTitle { get; set; }
        public string TitleResourceKey { get; set; }
        public int ColumnWidth { get; set; }
        public bool ShowTitle { get; set; }
        public RecordFilterGroup RecordFilter { get; set; }

        public virtual void ApplyTranslation(IRealTimeReportWidgetTranslationContext context)
        {


        }
        public abstract List<string> GetMeasureNames();
    }
    public interface IRealTimeReportWidgetTranslationContext
    {
        Guid LanguageId { get;}
        Guid AnalyticTableId { get; }
    }
    public class RealTimeReportWidgetTranslationContext : IRealTimeReportWidgetTranslationContext
    {
        public Guid LanguageId { get; set; }
        public Guid AnalyticTableId { get; set; }
    }
}
