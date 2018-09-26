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
        public string ReportName { get; set; }
        public Guid TableId { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public int? LastHours { get; set; }
        public int? CurrencyId { get; set; }

        public TimeGroupingUnit? TimeGroupingUnit { get; set; }

        public List<string> DimensionFields { get; set; }
        public List<string> ParentDimensions { get; set; }
        public List<string> MeasureFields { get; set; }
        public List<DimensionFilter> Filters { get; set; }
        public RecordFilterGroup FilterGroup { get; set; }

        public bool WithSummary { get; set; }
        public int? TopRecords { get; set; }

        public List<AnalyticQuerySubTable> SubTables { get; set; }

        public AnalyticQueryOrderType? OrderType { get; set; }

        public List<MeasureStyleRule> MeasureStyleRules { get; set; }

        public AnalyticQueryAdvancedOrderOptionsBase AdvancedOrderOptions { get; set; }
    }

    public class AnalyticQuerySubTable
    {
        public List<string> Dimensions { get; set; }

        public List<string> Measures { get; set; }

        public AnalyticQueryOrderType? OrderType { get; set; }

        public List<MeasureStyleRule> MeasureStyleRules { get; set; }

        public AnalyticQueryAdvancedOrderOptionsBase AdvancedOrderOptions { get; set; }
    }

    public abstract class AnalyticQueryAdvancedOrderOptionsBase
    {
        public virtual List<string> GetAdditionalMeasureNames()
        {
            return null;
        }
    }

    public class AnalyticQueryAdvancedMeasureOrderOptions : AnalyticQueryAdvancedOrderOptionsBase
    {
        public List<AnalyticQueryAdvancedMeasureOrderItem> MeasureOrders { get; set; }

        public override List<string> GetAdditionalMeasureNames()
        {
            if (this.MeasureOrders != null)
                return this.MeasureOrders.Select(itm => itm.MeasureName).ToList();
            else
                return null;
        }
    }

    public class AnalyticQueryAdvancedMeasureOrderItem
    {
        public string MeasureName { get; set; }

        public OrderDirection OrderDirection { get; set; }
    }
}
