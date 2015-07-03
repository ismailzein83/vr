using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.BP.Arguments
{
    public class ExecuteStrategyProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public List<int> StrategyIds { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int PeriodId { get; set; }

        public override string GetTitle()
        {
            return "Execute Strategy Process";
        }

        public override void MapExpressionValues(Dictionary<string, string> evaluatedExpressions)
        {
            if (evaluatedExpressions.ContainsKey("FromDate"))
            {
                FromDate = DateTime.Parse(evaluatedExpressions["FromDate"]);
            }

            if (evaluatedExpressions.ContainsKey("ToDate"))
            {
                ToDate = DateTime.Parse(evaluatedExpressions["ToDate"]);
            }
        }

    }
}
