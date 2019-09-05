using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcAggregationField
    {
        public string FieldName { get; set; }

        public string FieldTitle { get; set; }

        public RecordFilterGroup RecordFilter { get; set; }

        //ToBeDeleted
        public TimeRangeFilter TimeRangeFilter { get; set; }

        public DARecordAggregate RecordAggregate { get; set; }

        public VRTimePeriod TimePeriod { get; set; }
    }
}