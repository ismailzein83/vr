using System;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
        public class AccountCase
    {
        public int CaseID { get; set; }
        public string AccountNumber { get; set; }
        public int UserID { get; set; }
        public CaseStatus StatusID { get; set; }
        public DateTime StatusUpdatedTime { get; set; }
        public DateTime? ValidTill { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}
