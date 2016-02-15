using System;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountCaseLog
    {
        public int LogID { get; set; }

        public int? UserID { get; set; }

        public CaseStatus AccountCaseStatusID { get; set; }

        public DateTime StatusTime { get; set; }

        public string Reason { get; set; }
    }
}
