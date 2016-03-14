using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.BP.Arguments
{
    public class FindRelatedNumbersForNumberRangeProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string NumberPrefix { get; set; }

        public override string GetTitle()
        {
            return String.Format("Find Related Numbers For Number Prefix '{0}', Time Range ({1:dd-MMM-yy HH:mm} to {2:dd-MMM-yy HH:mm})", this.NumberPrefix, this.FromDate, this.ToDate);
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
