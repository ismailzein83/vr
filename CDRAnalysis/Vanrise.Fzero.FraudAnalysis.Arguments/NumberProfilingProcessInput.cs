using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Arguments
{
    public class NumberProfilingProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public NumberProfileParameters Parameters { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int PeriodId { get; set; }

        public override string GetTitle()
        {
            return String.Format("Number profiling ({0:dd-MMM-yy HH:mm}    to     {1:dd-MMM-yy HH:mm})", this.FromDate, this.ToDate);
        }

        public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {
            if (evaluatedExpressions.ContainsKey("ScheduleTime"))
            {
                if (PeriodId == (int)PeriodEnum.Hourly)
                {
                    FromDate = ((DateTime)evaluatedExpressions["ScheduleTime"]).AddDays(-1);
                    ToDate = (DateTime)evaluatedExpressions["ScheduleTime"];
                }
                else if (PeriodId == (int)PeriodEnum.Daily)
                {
                    FromDate = ((DateTime)evaluatedExpressions["ScheduleTime"]).AddHours(-1);
                    ToDate = (DateTime)evaluatedExpressions["ScheduleTime"];
                }

            }


        }

        public bool IncludeWhiteList { get; set; }

    }
}
