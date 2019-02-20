using System;
using TOne.WhS.Jazz.BP.Arguments;
using Vanrise.Common;

namespace TOne.WhS.Jazz.Business
{
    public class JazzReportBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            JazzReportProcessInput jazzReportProcessInputArg = context.IntanceToRun.InputArgument.CastWithValidate<JazzReportProcessInput>("context.IntanceToRun.InputArgument");

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                JazzReportProcessInput startedBPInstanceJazzReportArg = startedBPInstance.InputArgument as JazzReportProcessInput;
                if (startedBPInstanceJazzReportArg != null && jazzReportProcessInputArg.FromDate <= startedBPInstanceJazzReportArg.ToDate && jazzReportProcessInputArg.ToDate >= startedBPInstanceJazzReportArg.FromDate)
                {
                    context.Reason = "Another Jazz ERP Integration BP instance is running";
                    return false;
                }
            }

            return true;
        }

    }
}
