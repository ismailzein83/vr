using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class GenericSearchSettings : AnalyticReportSearchSettings
    {
        public List<GenericSearchSettingsDimension> Dimensions { get; set; }

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
