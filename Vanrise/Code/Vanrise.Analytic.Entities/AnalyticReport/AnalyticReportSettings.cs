using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticReportSettings
    {
        public List<int> AnalyticTableIds { get; set; }

        public List<AnalyticReportSearchSettings>  SearchSettings { get; set; }

        public List<AnalyticReportWidget> Widgets { get; set; }
    }
}
