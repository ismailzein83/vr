using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.BP.Arguments
{
    public class ExecuteStrategyProcessInput
    {
        public List<int> StrategyIds { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PeriodId { get; set; }
    }
}
