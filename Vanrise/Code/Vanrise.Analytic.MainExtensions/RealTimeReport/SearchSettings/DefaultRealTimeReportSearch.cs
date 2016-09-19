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

        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("A1CB1C46-0FFA-41B0-82B0-2CCE407AD86C"); } }
        public int TimeIntervalInMin { get; set; }
        public List<DefaultSearchSettingsFilter> Filters { get; set; }

    }
    public class DefaultSearchSettingsFilter
    {
        public string DimensionName { get; set; }

        public string Title { get; set; }
        public bool IsRequired { get; set; }
    }
}
