using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public enum TimeGroupingUnit { Day = 0, Hour = 1 }
    public class TimeVariationAnalyticQuery
    {
        public int TableId { get; set; }
        public DateTime FromTime { get; set; }
        
        public DateTime ToTime { get; set; }

        public TimeGroupingUnit TimeGroupingUnit { get; set; }

        public List<string> ParentDimensions { get; set; }
        public RecordFilterGroup FilterGroup { get; set; }
        public int? CurrencyId { get; set; }
        public List<string> MeasureFields { get; set; }
        public List<DimensionFilter> Filters { get; set; }
        public bool WithSummary { get; set; }
    }
}
