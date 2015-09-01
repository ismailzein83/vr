using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class AnalyticQuery
    {
        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }
        
        public AnalyticGroupField[] GroupFields { get; set; }

        public AnalyticMeasureField[] MeasureFields { get; set; }

        public List<DimensionFilter> Filters { get; set; }
    }

    public class DimensionFilter
    {
        public AnalyticGroupField Dimension { get; set; }

        public List<Object> FilterValues { get; set; }
    }
}
