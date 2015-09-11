using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountStatus
    {
        public string AccountNumber { get; set; }

        public CaseStatus Status { get; set; }

        public Dictionary<String, List<String>> AccountInfo { get; set; } //AccountNumberByIMEI
    }
}
