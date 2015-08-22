using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class BlockedLines
    {
        public string StrategyName { get; set; }

        public int BlockedLinesCount { get; set; }

        public DateTime? DateDay { get; set; }

        public List<string> AccountNumbers { get; set; }

             
    }
    
}