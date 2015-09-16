using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class CaseProductivityResultQuery
    {
        public List<int> StrategyIDs { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public bool GroupDaily { get; set; }

    }
}
