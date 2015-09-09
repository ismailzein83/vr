using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountSuspicionDetail
    {
        public long DetailID { get; set; }
        public string AccountNumber { get; set; }
        public SuspicionLevelEnum SuspicionLevelID { get; set; }
        public string StrategyName { get; set; }
        public CaseStatus AccountStatusID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
