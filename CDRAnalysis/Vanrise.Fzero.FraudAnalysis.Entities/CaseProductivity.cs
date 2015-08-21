using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class CaseProductivity
    {
        public string StrategyName { get; set; }

        public int GeneratedCases { get; set; }

        public int ClosedCases { get; set; }

        public int FraudCases { get; set; }

        public decimal ClosedoverGenerated { get; set; }

        public DateTime? DateDay { get; set; }

             
    }
    
}