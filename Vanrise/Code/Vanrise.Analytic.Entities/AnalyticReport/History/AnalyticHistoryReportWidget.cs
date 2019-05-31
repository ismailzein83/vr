using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public abstract class AnalyticHistoryReportWidget
    {
        public abstract Guid ConfigId { get; }
        public Guid AnalyticTableId { get; set; }
        public string WidgetTitle { get; set; }
        public string TitleResourceKey { get; set; }
        public int ColumnWidth { get; set; }
        public bool ShowTitle { get; set; }
        public RecordFilterGroup RecordFilter { get; set; }
        public virtual void ApplyTranslation(IAnalyticHistoryReportWidgetTranslationContext context) { }
        public abstract List<string> GetMeasureNames();
    }
    public interface IAnalyticHistoryReportWidgetTranslationContext
    {
        Guid LanguageId { get;}
        Guid AnalyticTableId { get;}
    }
    public class AnalyticHistoryReportWidgetTranslationContext : IAnalyticHistoryReportWidgetTranslationContext
    {
        public Guid LanguageId { get; set; }
        public Guid AnalyticTableId { get; set; }
    }
}
