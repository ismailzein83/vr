using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public enum AnalyticQueryOrderType { ByAllDimensions = 1, ByAllMeasures = 2, AdvancedMeasureOrder = 3 }
    public class AnalyticQuery
    {
        public int TableId { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime? ToTime { get; set; }

        public int? CurrencyId { get; set; }

        public TimeGroupingUnit? TimeGroupingUnit { get; set; }

        public List<string> DimensionFields { get; set; }
        public List<string> ParentDimensions { get; set; }
        public List<string> MeasureFields { get; set; }
        public List<DimensionFilter> Filters { get; set; }
        public RecordFilterGroup FilterGroup { get; set; }

        public bool WithSummary { get; set; }
        public int? TopRecords { get; set; }
        public AnalyticQueryOrderType? OrderType { get; set; }

        public List<MeasureStyleRule> MeasureStyleRules { get; set; }

        public AnalyticQueryAdvancedMeasureOrderOptions AdvanceMeasureOrderOptions { get; set; }
    }

    public class AnalyticQueryAdvancedMeasureOrderOptions
    {
        public List<AnalyticQueryAdvancedMeasureOrderItem> MeasureOrders { get; set; }
    }

    public class AnalyticQueryAdvancedMeasureOrderItem
    {
        public string MeasureName { get; set; }

        public OrderDirection OrderDirection { get; set; }
    }
}
