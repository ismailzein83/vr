using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;

namespace Vanrise.Analytic.MainExtensions.History.SearchSettings
{
    public class GenericSearchSettings : AnalyticHistoryReportSearchSettings
    {
        public override Guid ConfigId { get { return new Guid("BCC9AD0B-46EC-4ED1-B79F-47B4518F76B8"); } }

        public bool IsRequiredGroupingDimensions { get; set; }

        public bool ShowCurrency { get; set; }

        public List<GenericSearchSettingsDimension> GroupingDimensions { get; set; }

        public List<GenericSearchSettingsFilter> Filters { get; set; }

        public GenericSearchSettingsAdvancedFilters AdvancedFilters { get; set; }

        public bool ShowLegend { get; set; }

        public List<GenericSearchSettingsLegend> Legends { get; set; }

        public override void ApplyTranslation(IAnalyticHistoryReportTranslationContext context)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();

            if (GroupingDimensions != null && GroupingDimensions.Count > 0)
            {
                foreach (var dimension in GroupingDimensions)
                {
                    if (dimension.TitleResourceKey != null)
                        dimension.Title = vrLocalizationManager.GetTranslatedTextResourceValue(dimension.TitleResourceKey, dimension.Title, context.LanguageId);
                }
            }
            if (Filters != null && Filters.Count > 0)
            {
                foreach (var filter in Filters)
                {
                    if (filter.TitleResourceKey != null)
                        filter.Title = vrLocalizationManager.GetTranslatedTextResourceValue(filter.TitleResourceKey, filter.Title, context.LanguageId);
                }
            }
        }
    }

    public class GenericSearchSettingsDimension
    {
        public string DimensionName { get; set; }
        public string Title { get; set; }
        public string TitleResourceKey { get; set; }
        public bool IsSelected { get; set; }
    }

    public class GenericSearchSettingsFilter
    {
        public string DimensionName { get; set; }
        public string Title { get; set; }
        public string TitleResourceKey { get; set; }
        public bool IsRequired { get; set; }
    }

    public enum AdvancedFilterFieldsRelationType { AllFields = 0, SpecificFields = 1 }
    public enum AdvancedFilterMeasuresRelationType { AllFields = 0, SpecificMeasures = 1 }
    public class GenericSearchSettingsAdvancedFilters
    {
        public AdvancedFilterFieldsRelationType FieldsRelationType { get; set; }

        public List<AdvancedFilterField> AvailableFields { get; set; }

        public AdvancedFilterMeasuresRelationType? MeasuresRelationType { get; set; }

        public List<AdvancedFilterMeasure> AvailableMeasures { get; set; }
    }
    public class AdvancedFilterField
    {
        public string FieldName { get; set; }

        //public string FieldTitle { get; set; }
    }
    public class AdvancedFilterMeasure 
    {
        public string FieldName { get; set; }
    }

    public class GenericSearchSettingsLegend
    {
        public Guid AnalyticTableId { get; set; }
    }
}
