using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.BP.Arguments;
using Vanrise.Common;
namespace TOne.WhS.CodePreparation.Business
{
    public class CodePreperationProcessBPSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            CodePreparationInput inputArg = context.IntanceToRun.InputArgument.CastWithValidate<CodePreparationInput>("context.IntanceToRun.InputArgument");
            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                CodePreparationInput startedBPInstanceInputArg = startedBPInstance.InputArgument as CodePreparationInput;
                if (startedBPInstanceInputArg != null)
                {
                    if (startedBPInstanceInputArg.SellingNumberPlanId == inputArg.SellingNumberPlanId)
                    {
                        context.Reason = "Another process is running for the same Selling Number Plan";
                        return false;
                    }
                }
            }
            return true;
        }

    }
    
}
