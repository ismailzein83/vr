using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
   public class GenericAnalyticQuery
    {
        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public string Currency { get; set; }

        public AnalyticDimension[] DimensionFields { get; set; }

        public AnalyticMeasureField[] MeasureFields { get; set; }

        public List<DimensionFilter> Filters { get; set; }

        public bool WithSummary { get; set; }
        public int? TopRecords { get; set; }
    }
    public class DimensionFilter
    {
        public AnalyticDimension Dimension { get; set; }

        public List<Object> FilterValues { get; set; }
    }
}
