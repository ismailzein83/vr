using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class CancellingStrategyExecutionBatch
    {
        public List<long> StrategyExecutionItemIds;
        public List<int> AccountCaseIds;
    }
}
