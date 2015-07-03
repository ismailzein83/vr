using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

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
            if (evaluatedExpressions.ContainsKey("ScheduleTime"))
            {
                if ( PeriodId == (int)Enums.Period.Hour)
                {
                    FromDate = DateTime.Parse(evaluatedExpressions["ScheduleTime"]).AddDays(-1);
                    ToDate = DateTime.Parse(evaluatedExpressions["ScheduleTime"]);
                }
                else if (PeriodId == (int)Enums.Period.Day)
                {
                    FromDate = DateTime.Parse(evaluatedExpressions["ScheduleTime"]).AddHours(-1);
                    ToDate = DateTime.Parse(evaluatedExpressions["ScheduleTime"]);
                }
               
            }

           
        }

    }
}
