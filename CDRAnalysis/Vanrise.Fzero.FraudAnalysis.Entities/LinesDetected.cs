using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class LinesDetected
    {
        public string AccountNumber { get; set; }

        public decimal Volume { get; set; }

        public int ActiveDays { get; set; }

        public int Occurrences { get; set; }

        public string ReasonofBlocking { get; set; }

    }
    
}