using System;

namespace Retail.Cost.BP.Arguments
{
    public class CostEvaluatorProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return String.Format("Cost Evaluator Process");
        }
    }
}