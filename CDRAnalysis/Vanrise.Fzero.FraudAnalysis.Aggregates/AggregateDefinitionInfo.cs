using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public class AggregateDefinitionInfo
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public OperatorType OperatorTypeAllowed { get; set; }

        public string NumberPrecision { get; set; }

    }
}
