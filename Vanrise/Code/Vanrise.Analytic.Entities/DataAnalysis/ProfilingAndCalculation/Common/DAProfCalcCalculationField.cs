using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcCalculationField
    {
        public string FieldName { get; set; }

        public string FieldTitle { get; set; }

        public DataRecordFieldType FieldType { get; set; }

        public string Expression { get; set; }
    }

    public class DAProfCalcCalculationFieldDetail
    {
        public DAProfCalcCalculationField Entity { get; set; }

        public IDAProfCalcCalculationEvaluator Evaluator { get; set; }
    }
}