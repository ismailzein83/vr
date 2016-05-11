using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public enum TimeGroupingUnit { Minute = 0, Hour = 1}
    public class TimeVariationAnalyticQuery
    {
        public int TableId { get; set; }
        public DateTime FromTime { get; set; }
        
        public DateTime ToTime { get; set; }

        public TimeGroupingUnit TimeGroupingUnit { get; set; }

        public int? CurrencyId { get; set; }
        public List<string> MeasureFields { get; set; }
        public List<DimensionFilter> Filters { get; set; }
        public bool WithSummary { get; set; }
    }
}
