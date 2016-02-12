using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.BP.Arguments
{
    public class ExecuteStrategyForNumberRangeProcessOutput
    {
        public long CDRsProcessed { get; set; }

        public long NumberOfSubscribers { get; set; }

        public Dictionary<int, long> SuspicionsPerStrategy { get; set; }
    }
}
