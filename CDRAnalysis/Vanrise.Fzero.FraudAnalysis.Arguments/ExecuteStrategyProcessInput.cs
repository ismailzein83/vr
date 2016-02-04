using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Arguments
{
    public class ExecuteStrategyProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public List<int> StrategyIds { get; set; }

        public int PeriodId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool IncludeWhiteList { get; set; }

        public override string GetTitle()
        {
            IStrategyManager strategyManager = FraudManagerFactory.GetManager<IStrategyManager>();
            return String.Format("Execute Strategy Process ({0:dd-MMM-yy HH:mm} to {1:dd-MMM-yy HH:mm}), Strategies: {2}", this.FromDate, this.ToDate, String.Join(",", strategyManager.GetStrategyNames(StrategyIds)));
        }

        public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {
            if (evaluatedExpressions.ContainsKey("ScheduleTime"))
            {
                if (PeriodId == (int)PeriodEnum.Hourly)
                {
                    FromDate = ((DateTime)evaluatedExpressions["ScheduleTime"]).AddHours(-1);
                    ToDate = (DateTime)evaluatedExpressions["ScheduleTime"];
                }
                else if (PeriodId == (int)PeriodEnum.Daily)
                {
                    FromDate = ((DateTime)evaluatedExpressions["ScheduleTime"]).AddDays(-1);
                    ToDate = (DateTime)evaluatedExpressions["ScheduleTime"];
                }

            }
        }



    }
}
