using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class StrategyExecutionItem
    {
        public long ID { get; set; }

        public long StrategyExecutionID { get; set; }

        public string AccountNumber { get; set; }

        public int SuspicionLevelID { get; set; }

        public Dictionary<int,decimal?> FilterValues { get; set; }

        public SuspicionOccuranceStatus SuspicionOccuranceStatus { get; set; }

        public int? CaseID { get; set; }

        public Dictionary<String, Decimal> AggregateValues { get; set; }

        public HashSet<string> IMEIs { get; set; }

    }
}
