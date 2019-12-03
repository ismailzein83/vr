using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public enum DAProfCalcOutputFieldType { GroupingField = 0, AggregationField = 1, CalculationField = 2 }
    public class DAProfCalcOutputField
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public bool IsRequired { get; set; }

        public bool IsSelected { get; set; }

        public bool IsTechnical { get; set; }

        public DataRecordFieldType Type { get; set; }

        public DAProfCalcOutputFieldType DAProfCalcOutputFieldType { get; set; }
    }
}