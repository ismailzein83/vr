using System;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountCaseHistory
    {
        public int ID { get; set; }

        public int CaseID { get; set; }

        public int? UserID { get; set; }

        public CaseStatus Status { get; set; }

        public DateTime StatusTime { get; set; }

        public string Reason { get; set; }
    }
}
