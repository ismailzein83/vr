using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Arguments
{
    public class FindRelatedNumbersProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetTitle()
        {
            return String.Format("Find Related Numbers Process ({0:dd-MMM-yy HH:mm})", DateTime.Now);
        }

        public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {
            if (evaluatedExpressions.ContainsKey("ScheduleTime"))
            {
                FromDate = ((DateTime)evaluatedExpressions["ScheduleTime"]).AddDays(-1);
                ToDate = (DateTime)evaluatedExpressions["ScheduleTime"];
            }
        }

    }
}
