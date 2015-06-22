using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class VariationReportResult
    {
        public List<VariationReportsData> VariationReportsData { get; set; }
        public List<TimeRange> TimeRange;
    }
}
