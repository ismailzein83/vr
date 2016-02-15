using System;
using System.Collections.Generic;


namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class CaseProductivityQuery
    {
        public List<int> StrategyIDs { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public bool GroupDaily { get; set; }

    }
}
