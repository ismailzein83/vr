﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountSuspicionSummaryQuery
    {
        public string AccountNumber { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public List<int> StrategyIDs { get; set; }

        public List<CaseStatus> Statuses { get; set; }

        public List<SuspicionLevel> SuspicionLevelIDs { get; set; }
    }
}
