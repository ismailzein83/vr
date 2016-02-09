using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Arguments
{
    public class CancelStrategyExecutionProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public long StrategyExecutionId { get; set; }

        public int UserId { get; set; }

        public override string GetTitle()
        {
            return String.Format("Cancel Strategy Execution Id: {0}", StrategyExecutionId);
        }
    }
}
