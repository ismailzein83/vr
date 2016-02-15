
namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class StrategyExecutionDetail
    {
        public StrategyExecution Entity { get; set; }

        public string PeriodName { get; set; }

        public string StrategyName { get; set; }

        public string ExecutedByName { get; set; }

        public string CancelledByName { get; set; }

        public string StatusDescription { get; set; }
    }
}
