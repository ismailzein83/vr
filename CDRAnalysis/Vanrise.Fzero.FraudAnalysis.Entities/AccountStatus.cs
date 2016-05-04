using System;
namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountStatus
    {
        public string AccountNumber { get; set; }

        public CaseStatus Status { get; set; }

        public DateTime? ValidTill { get; set; }

        public string Reason { get; set; }

        public AccountStatusSource Source { get; set; }

        public int UserId { get; set; }

    }
}
