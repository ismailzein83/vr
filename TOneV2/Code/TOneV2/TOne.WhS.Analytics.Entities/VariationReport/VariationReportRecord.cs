using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public class VariationReportRecord
    {
        public VariationReportDimension? Dimension { get; set; }

        public Object DimensionId { get; set; }

        public string DimensionName { get; set; }

        public VariationReportRecordDimensionSuffix DimensionSuffix { get; set; }

        public List<Decimal> TimePeriodValues { get; set; }

        public decimal Average { get; set; }

        public decimal Percentage { get; set; }

        public decimal PreviousPeriodPercentage { get; set; }
    }
}
