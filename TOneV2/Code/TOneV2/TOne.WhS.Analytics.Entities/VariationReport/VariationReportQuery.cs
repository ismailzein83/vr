using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public class VariationReportQuery
    {
        public VariationReportType? ParentReportType { get; set; }

        public VariationReportType ReportType { get; set; }

        public DateTime ToDate { get; set; }

        public VariationReportTimePeriod TimePeriod { get; set; }

        public int NumberOfPeriods { get; set; }

        public List<VariationReportDimensionFilter> DimensionFilters { get; set; }
    }

    public enum VariationReportTimePeriod { Daily = 0, Weekly = 1, Monthly = 2 }

    public enum VariationReportType
    {
        InBoundMinutes = 0,
        OutBoundMinutes = 1,
        InOutBoundMinutes = 2,
        TopDestinationMinutes = 3,
        InBoundAmount = 4,
        OutBoundAmount = 5,
        InOutBoundAmount = 6,
        TopDestinationAmount = 7,
        Profit = 8
    }

    public enum VariationReportDimension { Customer = 0, Supplier = 1, Zone = 2 }

    public class VariationReportDimensionFilter
    {
        public VariationReportDimension Dimension { get; set; }

        public List<Object> FilterValues { get; set; }
    }
}
