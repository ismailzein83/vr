using System;
using System.Collections;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountStatusQuery
    {
        public CaseStatus Status { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string AccountNumber { get; set; }
        public List<AccountStatusSource> Sources { get; set; }
        public List<int> UserIds { get; set; }
    }
}
