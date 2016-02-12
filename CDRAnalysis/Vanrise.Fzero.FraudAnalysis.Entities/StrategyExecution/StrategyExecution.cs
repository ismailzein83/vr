using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class StrategyExecution
    {
        public long ID { get; set; }

        public long ProcessID { get; set; }

        public int StrategyID { get; set; }

        public int PeriodID { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime ExecutionDate { get; set; }

        public DateTime? CancellationDate { get; set; }

        public int ExecutedBy { get; set; }

        public int? CancelledBy { get; set; }

        public long? NumberofSubscribers { get; set; }

        public long? NumberofCDRs { get; set; }

        public long? NumberofSuspicions { get; set; }

        public double? ExecutionDuration { get; set; }

        public SuspicionOccuranceStatus Status { get; set; }

    }
}
