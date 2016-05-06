using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticHistoryReportSettings : AnalyticReportSettings
    {
        public List<int> AnalyticTableIds { get; set; }

        public AnalyticHistoryReportSearchSettings SearchSettings { get; set; }

        public List<AnalyticHistoryReportWidget> Widgets { get; set; }
    }
}
