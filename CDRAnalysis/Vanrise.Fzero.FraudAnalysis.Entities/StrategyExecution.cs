using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class StrategyExecution
    {
        public int ID { get; set; }

        public int ProcessID { get; set; }

        public int StrategyID { get; set; }

        public int PeriodID { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime ExecutionDate { get; set; }
    }
}
