using System;
using Vanrise.Common;
using Vanrise.GenericData.BP.Arguments;

namespace Vanrise.GenericData.MainExtensions
{
    public class CDRCorrelationBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            CDRCorrelationProcessInput inputArg = context.IntanceToRun.InputArgument.CastWithValidate<CDRCorrelationProcessInput>("context.IntanceToRun.InputArgument");

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                CDRCorrelationProcessInput startedBPInstanceCDRCorrelationArg = startedBPInstance.InputArgument as CDRCorrelationProcessInput;
                if (startedBPInstanceCDRCorrelationArg != null)
                {
                    context.Reason = String.Format("Another CDR Correlation instance is running");
                    return false;
                }
            }

            return true;
        }
    }
}