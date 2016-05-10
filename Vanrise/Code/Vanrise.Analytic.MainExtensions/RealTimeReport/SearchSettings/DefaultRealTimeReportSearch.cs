using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.RealTimeReport.SearchSettings
{
    public class DefaultRealTimeReportSearch: RealTimeReportSearchSettings 
    {
        public List<DefaultSearchSettingsFilter> Filters { get; set; }

    }
    public class DefaultSearchSettingsFilter
    {
        public string DimensionName { get; set; }

        public string Title { get; set; }
        public bool IsRequired { get; set; }
    }
}
