using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.Analytic.MainExtensions.History.SearchSettings;
using Vanrise.Common.Business;

namespace Vanrise.Analytic.MainExtensions.RealTimeReport.SearchSettings
{
    public class DefaultRealTimeReportSearch : RealTimeReportSearchSettings
    {
        public override Guid ConfigId { get { return new Guid("A1CB1C46-0FFA-41B0-82B0-2CCE407AD86C"); } }
        public int TimeIntervalInMin { get; set; }
        public List<DefaultSearchSettingsFilter> Filters { get; set; }

        GenericSearchSettingsAdvancedFilters advancedFilters;
        public GenericSearchSettingsAdvancedFilters AdvancedFilters
        {
            get
            {
                if (advancedFilters == null)
                {
                    advancedFilters = new GenericSearchSettingsAdvancedFilters()
                    {
                        FieldsRelationType = History.SearchSettings.AdvancedFilterFieldsRelationType.AllFields
                    };
                }
                return advancedFilters;
            }
            set
            {
                advancedFilters = value;
            }
        }

        public override void ApplyTranslation(IAnalyticRealTimeReportTranslationContext context)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();

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

    public class DefaultSearchSettingsFilter
    {
        public string DimensionName { get; set; }
        public string Title { get; set; }
        public string TitleResourceKey { get; set; }
        public bool IsRequired { get; set; }
    }
}
