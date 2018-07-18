using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.BP.Arguments
{
    public class DBReplicationProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public static Guid BPDefinitionId { get { return new Guid("29CFE0E9-A2B9-4692-B787-DC6F62B1C2A0"); } }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public DBReplicationSettings Settings { get; set; }

        public Guid DBReplicationDefinitionId { get; set; }

        public int NumberOfDaysPerInterval { get; set; }

        public override string GetTitle()
        {
            return String.Format("Database Replication from {0} to {1}", FromTime.ToString("yyyy-MM-dd"), ToTime.ToString("yyyy-MM-dd"));
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