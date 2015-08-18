using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountCaseType
    {
        public string AccountNumber { get; set; }
        public int StrategyId { get; set; }
        public int SuspicionLevelID { get; set; }
    }
}
