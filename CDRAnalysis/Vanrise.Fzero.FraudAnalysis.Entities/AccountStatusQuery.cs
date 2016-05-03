using System;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountStatusQuery
    {
        public CaseStatus Status { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string AccountNumber { get; set; }
    }
}
