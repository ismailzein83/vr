using System;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountSuspicionSummary
    {
        public string AccountNumber { get; set; }
        public SuspicionLevel SuspicionLevelID { get; set; }
        public int NumberOfOccurances { get; set; }
        public DateTime? LastOccurance { get; set; }
        public CaseStatus Status { get; set; }
        public int CaseID { get; set; }
    }
}
