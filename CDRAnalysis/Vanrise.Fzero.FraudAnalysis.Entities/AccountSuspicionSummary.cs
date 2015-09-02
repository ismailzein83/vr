using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountSuspicionSummary
    {
        public string AccountNumber { get; set; }
        public SuspicionLevel SuspicionLevelID { get; set; }
        public string StrategyName { get; set; }
        public int NumberOfOccurances { get; set; }
        public CaseStatus CaseStatusID { get; set; }
    }
}
