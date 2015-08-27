using Vanrise.Fzero.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AggregateDefinitionInfo
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public CommonEnums.OperatorType OperatorTypeAllowed { get; set; }

    }
}
