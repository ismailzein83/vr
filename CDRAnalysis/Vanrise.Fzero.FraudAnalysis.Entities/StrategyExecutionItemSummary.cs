using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class StrategyExecutionItemSummary
    {
        public string AccountNumber { get; set; }

        public HashSet<string> IMEIs { get; set; }

    }
}
