using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountCaseUpdateResultQuery
    {
        public string AccountNumber { get; set; }
        public CaseStatus CaseStatus { get; set; }
        public DateTime? ValidTill { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
