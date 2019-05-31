using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Analytic.MainExtensions.History.Widgets
{
    public class AnalyticGridWidget : AnalyticHistoryReportWidget
    {
        public override Guid ConfigId { get { return new Guid("7A2A35E2-543A-42C7-B97F-E05EE8D09A00"); } }
        public bool RootDimensionsFromSearchSection { get; set; }

        public List<AnalyticGridWidgetDimension> Dimensions { get; set; }

        public List<AnalyticGridWidgetMeasure> Measures { get; set; }

        public List<Entities.AnalyticItemAction> ItemActions { get; set; }

        public List<GridSubTablesDefinition> SubTables { get; set; }

        public AnalyticQueryOrderType OrderType { get; set; }

        public Object AdvancedOrderOptions { get; set; }

        public bool WithSummary { get; set; }

        public bool AutoRefresh { get; set; }

        public int? AutoRefreshInterval { get; set; }

        public override List<string> GetMeasureNames()
        {
            return this.Measures.Select(measure => measure.MeasureName).ToList();
        }
        public override void ApplyTranslation(IAnalyticHistoryReportWidgetTranslationContext context)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();

            if (Dimensions != null && Dimensions.Count > 0)
            {
                foreach (var dimension in Dimensions)
                {
                    if (dimension.TitleResourceKey != null)
                        dimension.Title = vrLocalizationManager.GetTranslatedTextResourceValue(dimension.TitleResourceKey, dimension.Title, context.LanguageId);
                }
            }
            if (Measures != null && Measures.Count > 0)
            {
                foreach (var measure in Measures)
                {
                    if (measure.TitleResourceKey != null)
                        measure.Title = vrLocalizationManager.GetTranslatedTextResourceValue(measure.TitleResourceKey, measure.Title, context.LanguageId);
                }
            }
            if (SubTables != null && SubTables.Count > 0)
            {
                foreach (var subTable in SubTables)
                {
                    if (subTable != null && subTable.Measures != null && subTable.Measures.Count > 0)
                    {
                        foreach (var measure in subTable.Measures)
                        {
                            if (measure.TitleResourceKey != null)
                                measure.Title = vrLocalizationManager.GetTranslatedTextResourceValue(measure.TitleResourceKey, measure.Title, context.LanguageId);
                        }
                    }
                }
            }
        }
    }

    public class AnalyticGridWidgetDimension
    {
        public string DimensionName { get; set; }
        public string Title { get; set; }
        public string TitleResourceKey { get; set; }
        public bool IsRootDimension { get; set; }
        public GridColumnSettings ColumnSettings { get; set; }
        public Guid? ColumnStyleId { get; set; }
    }

    public class AnalyticGridWidgetMeasure
    {
        public string MeasureName { get; set; }
        public string Title { get; set; }
        public string TitleResourceKey { get; set; }
        public GridColumnSettings ColumnSettings { get; set; }
        public Guid? ColumnStyleId { get; set; }
        public bool IsHidden { get; set; }
        public bool IsHiddenInListView { get; set; }

    }

    public enum SubTablePositionEnum { BeforeAllMeasures = 1, AfterAllMeasures = 2, BeforeSpecificMeasure = 3, AfterSpecificMeasure = 4 }

    public class GridSubTablesDefinition
    {
        public List<string> Dimensions { get; set; }
        public List<AnalyticGridSubtabletMeasure> Measures { get; set; }
        public SubTablePosition SubTablePosition { get; set; }
    }

    public class AnalyticGridSubtabletMeasure
    {
        public string MeasureName { get; set; }
        public string Title { get; set; }
        public string TitleResourceKey { get; set; }
        public GridColumnSettings ColumnSettings { get; set; }
        public Guid? ColumnStyleId { get; set; }

    }

    public class SubTablePosition
    {
        public SubTablePositionEnum PositionValue { get; set; }
        public string ReferenceMeasure { get; set; }
    }
}