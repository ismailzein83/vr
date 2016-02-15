﻿using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.CDRImport.BP.Arguments
{
    public class StagingtoCDRProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }


        public override string GetTitle()
        {
           return String.Format("Staging to CDR Process ({0:dd-MMM-yy HH:mm}    to     {1:dd-MMM-yy HH:mm})", this.FromDate, this.ToDate);
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
