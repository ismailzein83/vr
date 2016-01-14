﻿using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.BP.Arguments
{
    public class FillDataWarehouseProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetTitle()
        {
            return String.Format("Fill Data Warehouse: ({0:dd-MMM-yy HH:mm} to {1:dd-MMM-yy HH:mm})", FromDate, ToDate);
        }

        public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {
            FromDate = ((DateTime)evaluatedExpressions["ScheduleTime"]).AddDays(-1);
            ToDate = (DateTime)evaluatedExpressions["ScheduleTime"];
        }

    }
}
