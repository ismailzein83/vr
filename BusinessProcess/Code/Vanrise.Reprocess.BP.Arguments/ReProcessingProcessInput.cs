using System;
using System.Collections.Generic;
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
            return String.Format("{0} from {1} to {2}", reprocessName, FromTime.ToString("yyyy-MM-dd HH:mm:ss"), ToTime.ToString("yyyy-MM-dd HH:mm:ss"));
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
