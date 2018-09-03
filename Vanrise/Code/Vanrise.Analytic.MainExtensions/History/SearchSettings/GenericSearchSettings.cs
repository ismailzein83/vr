using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;

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

        public List<GenericSearchSettingsLegend> Legends { get; set; }
    }

    public class GenericSearchSettingsDimension
    {
        public string DimensionName { get; set; }

        public bool IsSelected { get; set; }
    }

    public class GenericSearchSettingsFilter
    {
        public string DimensionName { get; set; }

        public string Title { get; set; }

        public bool IsRequired { get; set; }
    }

    public enum AdvancedFilterFieldsRelationType { AllFields = 0, SpecificFields = 1 }
    public class GenericSearchSettingsAdvancedFilters
    {
        public AdvancedFilterFieldsRelationType FieldsRelationType { get; set; }

        public List<AdvancedFilterField> AvailableFields { get; set; }
    }
    public class AdvancedFilterField
    {
        public string FieldName { get; set; }

        //public string FieldTitle { get; set; }
    }

    public class GenericSearchSettingsLegend
    {
        public string Header { get; set; }

        public string Content { get; set; }
    }
}
