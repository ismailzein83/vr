
using Vanrise.Fzero.Entities;
namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AggregateDefinition
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IAggregate Aggregation { get; set; }

        public OperatorTypeEnum OperatorTypeAllowed { get; set; }

    }
}
