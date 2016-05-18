using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticRecord
    {
        public DateTime? Time { get; set; }

        public DimensionValue[] DimensionValues { get; set; }

        public MeasureValues MeasureValues { get; set; }
    }
}
