using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticQuery
    {
        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public string Currency { get; set; }

        public List<string> DimensionFields { get; set; }

        public List<string> MeasureFields { get; set; }

        public List<DimensionFilter> Filters { get; set; }
        public int? TopRecords { get; set; }
    }
}
