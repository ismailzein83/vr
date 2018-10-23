using System;
using System.Collections.Generic;

namespace TOne.WhS.Analytics.Entities
{
    public class VariationReportQuery
    {
        public VariationReportType ReportType { get; set; }

        public IEnumerable<ParentDimension> ParentDimensions { get; set; }

        public DateTime ToDate { get; set; }

        public VariationReportTimePeriod TimePeriod { get; set; }

        public int NumberOfPeriods { get; set; }

        public bool GroupByProfile { get; set; }
        public bool OnlyTotal { get;set; }

        public int? CurrencyId { get; set; }

        //public List<VariationReportDimensionFilter> DimensionFilters { get; set; }
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
        Profit = 8,
        OutBoundProfit = 9,
        TopDestinationProfit = 10
    }

    public enum VariationReportDimension { Customer = 0, Supplier = 1, Zone = 2 }

    public enum VariationReportRecordDimensionSuffix
    {
        None = 0,
        In = 1,
        Out = 2,
        Total = 3
    }

    public class VariationReportDimensionFilter
    {
        public VariationReportDimension Dimension { get; set; }

        public List<Object> FilterValues { get; set; }
    }

    public class ParentDimension
    {
        public VariationReportDimension Dimension { get; set; }

        public object Value { get; set; }
    }
}
