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
    }
}
