using System;
using Vanrise.BusinessProcess.Entities;
using Retail.BusinessEntity.BP.Arguments;

namespace Retail.BusinessEntity.Business
{
    public class AccountRecurringChargeEvaluatorBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(IBPDefinitionCanRunBPInstanceContext context)
        {
            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                var startedBPInstanceReprocessArg = startedBPInstance.InputArgument as AccountRecurringChargeEvaluatorProcessInput;
                if (startedBPInstanceReprocessArg != null)
                {
                    context.Reason = "Another Account Recurring Charge Evaluator instance is running";
                    return false;
                }
            }
            return true;
        }
    }
}
