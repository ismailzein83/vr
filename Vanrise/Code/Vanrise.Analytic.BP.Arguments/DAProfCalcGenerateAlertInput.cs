using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.Analytic.BP.Arguments
{
    public class DAProfCalcGenerateAlertInput : BaseProcessInputArgument
    {
        public Guid AlertRuleTypeId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public DAProfCalcChunkTimeEnum? ChunkTime { get; set; }

        public override string GetTitle()
        {
            return String.Format("Data Analysis Profiling And Calculation Generate Alert Process from {0} to {1}", FromTime.ToString("yyyy-MM-dd HH:mm:ss"), ToTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {
            if (evaluatedExpressions.ContainsKey("ScheduleTime") && evaluatedExpressions.ContainsKey("DaysBack") && evaluatedExpressions.ContainsKey("NumberOfDays"))
            {
                var effectiveDate = (DateTime)evaluatedExpressions["ScheduleTime"];
                this.FromTime = effectiveDate.Date.AddDays(-1 * int.Parse(evaluatedExpressions["DaysBack"].ToString()));
                this.ToTime = this.FromTime.AddDays(int.Parse(evaluatedExpressions["NumberOfDays"].ToString()));
            }
        }
    }
}