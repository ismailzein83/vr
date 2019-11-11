using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcAggregationField
    {
        public string FieldName { get; set; }

        public string FieldTitle { get; set; }

        public DARecordAggregate RecordAggregate { get; set; }

        public RecordFilterGroup RecordFilter { get; set; }

        public DAAggregateTimeFilter TimeFilter { get; set; }

        public string Expression { get; set; }

        //ToBeDeleted
        public TimeRangeFilter TimeRangeFilter { get; set; }
    }

    public class DAAggregateTimeFilter
    {
        public VRTimePeriod TimePeriod { get; set; }

        public bool ExcludeFrom { get; set; }

        public bool ExcludeTo { get; set; }
    }

    public class DAProfCalcAggregationFieldDetail
    {
        public DAProfCalcAggregationField Entity { get; set; }

        public IDAProfCalcAggregationEvaluator Evaluator { get; set; }
    }
}