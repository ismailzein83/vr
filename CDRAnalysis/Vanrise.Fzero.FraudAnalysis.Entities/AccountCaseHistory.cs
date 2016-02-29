using System;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountCaseHistory
    {
        public int AccountCaseHistoryId { get; set; }

        public int CaseId { get; set; }

        public int? UserId { get; set; }

        public CaseStatus Status { get; set; }

        public DateTime StatusTime { get; set; }

        public string Reason { get; set; }
    }
}
