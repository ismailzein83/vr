using System;
using System.Collections.Generic;
using Vanrise.Reprocess.Entities;

namespace Vanrise.Reprocess.BP.Arguments
{
    public class ReProcessingProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid ReprocessDefinitionId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public ReprocessChunkTimeEnum ChunkTime { get; set; }

        public override string GetTitle()
        {
            return String.Format("Reprocess from {0} to {1}", FromTime, ToTime);
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
