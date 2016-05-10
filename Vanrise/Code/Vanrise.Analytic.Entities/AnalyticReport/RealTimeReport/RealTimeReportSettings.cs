using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class RealTimeReportSettings : AnalyticReportSettings
    {
        public List<int> AnalyticTableIds { get; set; }
        public RealTimeReportSearchSettings SearchSettings { get; set; }
        public List<RealTimeReportWidget> Widgets { get; set; }

    }
}
