using System;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountCase
    {
        public string AccountNumber { get; set; }

        public int StatusID { get; set; }

        public DateTime? ValidTill { get; set; }

        public int? StrategyId { get; set; }

        public int? UserId { get; set; }

        public DateTime LogDate { get; set; }

        public string StrategyName { get; set; }

        public string StatusName { get; set; }

        public string UserName { get; set; }

        public int? SuspicionLevelID { get; set; }

        public string SuspicionLevelName { get; set; }
    }

    public class AccountCase1
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
