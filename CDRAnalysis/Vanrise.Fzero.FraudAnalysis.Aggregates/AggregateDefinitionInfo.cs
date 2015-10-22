using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public class AggregateDefinitionInfo
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string KeyName { get; set; }

        public OperatorType OperatorTypeAllowed { get; set; }

        public string NumberPrecision { get; set; }

    }
}
