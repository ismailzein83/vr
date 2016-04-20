using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.SearchSettings
{
    public class GenericSearchSettings : AnalyticReportSearchSettings
    {
        public List<GenericSearchSettingsDimension> GroupingDimensions { get; set; }

        public List<GenericSearchSettingsFilter> Filters { get; set; }
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

        public GenericData.Entities.DataRecordFieldType FieldType { get; set; }

        public bool IsRequired { get; set; }
    }
}
