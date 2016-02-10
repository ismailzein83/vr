﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;


namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class StrategyExecutionQuery
    {

        public DateTime? FromCDRDate { get; set; }

        public DateTime? ToCDRDate { get; set; }

        public DateTime? FromExecutionDate { get; set; }

        public DateTime? ToExecutionDate { get; set; }

        public DateTime? FromCancellationDate { get; set; }

        public DateTime? ToCancellationDate { get; set; }

        public List<int> StrategyIds { get; set; }

        public List<int> UserIds { get; set; }

        public int? PeriodId { get; set; }

        public List<int> StatusIds { get; set; }

    }
}
