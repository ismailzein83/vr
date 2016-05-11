using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class TimeVariationAnalyticRecord
    {
        public DateTime Time { get; set; }

        public MeasureValues MeasureValues { get; set; }
    }
}
