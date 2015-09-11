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

        public SuspicionLevel SuspicionLevelID { get; set; }

        public string StrategyName { get; set; }

        public SuspicionOccuranceStatus SuspicionOccuranceStatus { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime ExecutionDate { get; set; }

        public Dictionary<string, decimal> AggregateValues { get; set; }
    }
}
