using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.Reprocess.Entities;

namespace Vanrise.Reprocess.BP.Arguments
{
    public class ReProcessingProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public static Guid BPDefinitionId { get { return new Guid("2E5D1E80-FE3F-403B-83ED-0232C84D6DD1"); } }

        public Guid ReprocessDefinitionId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public ReprocessChunkTimeEnum ChunkTime { get; set; }

        public bool UseTempStorage { get; set; }

        public bool IgnoreSynchronisation { get; set; }

        public ReprocessFilter Filter { get; set; }

        public override string GetTitle()
        {
            IReprocessDefinitionManager reprocessDefinitionManager = ReprocessManagerFactory.GetManager<IReprocessDefinitionManager>();
            string reprocessName = reprocessDefinitionManager.GetReprocessDefinition(ReprocessDefinitionId).Name;
            string title = $"{reprocessName} from {FromTime.ToString("yyyy-MM-dd HH:mm:ss")} to {ToTime.ToString("yyyy-MM-dd HH:mm:ss")}";

            if (Filter != null)
                title = string.Concat(title, " with Filter");

            return title;
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
