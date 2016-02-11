﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountCaseLog
    {
        public int LogID { get; set; }

        public int? UserID { get; set; }

        public CaseStatusEnum AccountCaseStatusID { get; set; }

        public DateTime StatusTime { get; set; }

        public string Reason { get; set; }
    }
}
