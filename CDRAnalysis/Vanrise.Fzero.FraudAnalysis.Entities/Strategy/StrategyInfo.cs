using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class StrategyInfo
    {
        public int Id { get; set; }

        public int PeriodId { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }
    }
}