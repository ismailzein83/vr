using System;
using System.Collections.Generic;


namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountThreshold
    {
        public DateTime DateDay { get; set; }

        public Dictionary<int, Decimal> CriteriaValues { get; set; }

        public string SuspicionLevelName { get; set; }

        public string StrategyName { get; set; }

    }
}
