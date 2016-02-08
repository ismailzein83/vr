﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class CancellingStrategyExecutionBatch
    {
        public List<long> StrategyExecutionItemIds;
        public List<int> AccountCaseIds;
    }
}
