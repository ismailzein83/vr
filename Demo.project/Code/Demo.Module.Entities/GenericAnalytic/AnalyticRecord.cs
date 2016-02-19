using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class AnalyticRecord
    {
        public AnalyticDimensionValue[] DimensionValues { get; set; }

        //public Object[] MeasureValues { get; set; }

        public Dictionary<AnalyticMeasureField,Object> MeasureValues { get; set; }
    }
}
