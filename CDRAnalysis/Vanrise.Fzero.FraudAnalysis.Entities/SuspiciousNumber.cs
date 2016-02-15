using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class SuspiciousNumber
    {
        public string Number { get; set; }

        public int SuspicionLevel { get; set; }

        public Dictionary<int,Decimal> CriteriaValues { get; set; }

        public DateTime? DateDay { get; set; }
        
        public int StrategyId { get; set; }
    }
}
