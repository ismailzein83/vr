﻿using System;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class DWAccountCase
    {
        public int CaseID { get; set; }
        public CaseStatus CaseStatus { get; set; }
        public NetType NetType { get; set; }
        public int StrategyID { get; set; }
        public bool IsDefault { get; set; }
        public PeriodEnum PeriodID { get; set; }
        public SuspicionLevel SuspicionLevel { get; set; }
        public DateTime CaseGenerationTime { get; set; }
        public int StrategyUser { get; set; }
        public int? CaseUser { get; set; }
        public string AccountNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
