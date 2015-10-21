
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public class AggregateDefinition
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IAggregate Aggregation { get; set; }

        public OperatorType OperatorTypeAllowed { get; set; }

    }
}
