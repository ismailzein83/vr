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

        public IEnumerable<string> DimensionFields { get; set; }

        public IEnumerable<string> MeasureFields { get; set; }

        public List<DimensionFilter> Filters { get; set; }

    }
}
