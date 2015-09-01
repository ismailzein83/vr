using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class AnalyticRecord
    {
        public AnalyticDimensionValue[] DimensionValues { get; set; }

        public Object[] MeasureValues { get; set; }
    }
}
