using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class CancelAccountCasesQuery
    {
        public string AccountNumber { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public List<int> StrategyIDs { get; set; }
    }
}
