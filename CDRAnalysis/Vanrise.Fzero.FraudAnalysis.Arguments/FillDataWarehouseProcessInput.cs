using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.BP.Arguments
{
    public class FillDataWarehouseProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetTitle()
        {
            return String.Format("Fill Data Warehouse: ({0:dd-MMM-yy HH:mm})", DateTime.Now);
        }

        public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {

        }

    }
}
