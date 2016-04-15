using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public class VariationReportPeriodValue
    {
        public Object DimensionId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public Decimal Value { get; set; }
    }
}
