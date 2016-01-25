﻿using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class NumberProfile
    {
        public NumberProfile()
        {
            AggregateValues = new Dictionary<string, decimal>();
        }

        public Dictionary<String, Decimal> AggregateValues { get; set; }

        public String AccountNumber { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int StrategyId { get; set; }

        public string StrategyName { get; set; }

        public long StrategyExecutionID { get; set; }

        public HashSet<string> IMEIs { get; set; }


    }
}
