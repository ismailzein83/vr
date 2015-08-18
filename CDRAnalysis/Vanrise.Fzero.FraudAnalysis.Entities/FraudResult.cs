using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class FraudResult
    {
        public DateTime LastOccurance { get; set; }

        public string AccountNumber { get; set; }

        public Enums.SuspicionLevel SuspicionLevel { get; set; }

        public string SuspicionLevelName { get; set; }

        public string StrategyName { get; set; }

        public string NumberofOccurances { get; set; }

        public string CaseStatus { get; set; }

        public int? StatusId { get; set; }

        public DateTime? ValidTill { get; set; }

    }

    
}