﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountCaseUpdateQuery
    {
        public int CaseId { get; set; }

        public string AccountNumber { get; set; }

        public CaseStatus CaseStatus { get; set; }

        public DateTime? ValidTill { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Reason { get; set; }
    }
}
