
namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AggregateDefinition
    {

        public int AggregateId { get; set; }

        public string Name { get; set; }

        public IAggregate Aggregation { get; set; }

    }
}
