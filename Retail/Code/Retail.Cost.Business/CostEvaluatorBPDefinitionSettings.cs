using System;
using Retail.Cost.BP.Arguments;

namespace Retail.Cost.Business
{
    public class CostEvaluatorBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                CostEvaluatorProcessInput startedBPInstanceCostEvaluatorArg = startedBPInstance.InputArgument as CostEvaluatorProcessInput;
                if (startedBPInstanceCostEvaluatorArg != null)
                {
                    context.Reason = String.Format("Another Cost Evaluator instance is running");
                    return false;
                }
            }

            return true;
        }
    }
}