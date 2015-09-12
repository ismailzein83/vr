using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class StrategyExecutionDetailSummary
    {
        public string AccountNumber { get; set; }

        public HashSet<string> IMEIs { get; set; }

    }
}
