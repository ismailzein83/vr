using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Arguments
{
    public class FindRelatedNumbersProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return String.Format("Find Related Numbers Process ({0:dd-MMM-yy HH:mm})", DateTime.Now);
        }

        public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {

        }

    }
}
