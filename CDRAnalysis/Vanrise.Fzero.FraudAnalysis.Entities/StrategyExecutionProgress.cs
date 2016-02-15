using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class StrategyExecutionProgress
    {
        public long CDRsProcessed { get; set; }

        public long NumberOfSubscribers { get; set; }

        public Dictionary<int, long> SuspicionsPerStrategy { get; set; }
    }
}

