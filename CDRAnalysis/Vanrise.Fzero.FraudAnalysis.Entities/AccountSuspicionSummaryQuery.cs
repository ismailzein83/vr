using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountSuspicionSummaryQuery
    {
        public string AccountNumber { get; set; }

        public DateTime ExecutionDate { get; set; }

        public List<int> StrategyIDs { get; set; }

        public List<CaseStatus> AccountStatusIDs { get; set; }

        public List<SuspicionLevel> SuspicionLevelIDs { get; set; }
    }
}
