using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class StrategyExecutionDetail
    {
        public StrategyExecution Entity { get; set; }

        public string PeriodName { get; set; }

        public string StrategyName { get; set; }

        public BPInstanceStatus Status { get; set; }

        public string ExecutedByName { get; set; }

        public string CancelledByName { get; set; }
    }
}
