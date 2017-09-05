using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Mediation.Generic.BP.Arguments;
using Mediation.Generic.Entities;
using Vanrise.Queueing.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Mediation.Generic.Business
{
    class MediationBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override void OnBPExecutionCompleted(IBPDefinitionBPExecutionCompletedContext context)
        {
            HoldRequestManager holdRequestManager = new HoldRequestManager();
            holdRequestManager.DeleteHoldRequestByBPInstanceId(context.BPInstance.ProcessInstanceID);
        }
        public override bool CanRunBPInstance(IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            MediationProcessInput mediationProcessInput = context.IntanceToRun.InputArgument.CastWithValidate<MediationProcessInput>("context.IntanceToRun.InputArgument");
            MediationDefinitionManager manager = new MediationDefinitionManager();
            MediationDefinition mediationDefinition = manager.GetMediationDefinition(mediationProcessInput.MediationDefinitionId);
            mediationDefinition.ThrowIfNull("mediationDefinition", mediationProcessInput.MediationDefinitionId);

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                MediationProcessInput startedBPInstanceReprocessArg = startedBPInstance.InputArgument as MediationProcessInput;
                if (startedBPInstanceReprocessArg != null)
                {
                    MediationDefinition startedMediationDefinition = manager.GetMediationDefinition(startedBPInstanceReprocessArg.MediationDefinitionId);
                    startedMediationDefinition.ThrowIfNull("startedMediationDefinition", startedBPInstanceReprocessArg.MediationDefinitionId);

                    if (mediationDefinition.MediationDefinitionId == startedMediationDefinition.MediationDefinitionId)
                    {
                        context.Reason = String.Format("Another mediation instance is running of same mediation definition '{0}'", mediationDefinition.Name);
                        return false;
                    }
                }
            }
            //HoldRequestManager holdRequestManager = new HoldRequestManager();
            //HoldRequest existingHoldRequest = holdRequestManager.GetHoldRequest(context.IntanceToRun.ProcessInstanceID);
            //if (existingHoldRequest == null)
            //{
            //    QueueExecutionFlowDefinitionManager queueExecutionFlowDefinitionManager = new QueueExecutionFlowDefinitionManager();
            //    QueueExecutionFlowDefinition queueExecutionFlowDefinition = queueExecutionFlowDefinitionManager.GetExecutionFlowDefinition(mediationDefinition.ExecutionFlowDefinitionId);
            //    queueExecutionFlowDefinition.ThrowIfNull("queueExecutionFlowDefinition", mediationDefinition.ExecutionFlowDefinitionId);

            //    DateTimeRange dateTimeRange = new UtilityManager().GetDateTimeRange();

            //    holdRequestManager.InsertHoldRequest(context.IntanceToRun.ProcessInstanceID, mediationDefinition.ExecutionFlowDefinitionId, dateTimeRange.From, dateTimeRange.To,
            //        queueExecutionFlowDefinition.Stages.Select(itm => itm.StageName).ToList(), null, HoldRequestStatus.Pending);

            //    context.Reason = "Waiting CDR Import";
            //    return false;
            //}
            //else if (existingHoldRequest.Status != HoldRequestStatus.CanBeStarted)
            //{
            //    context.Reason = "Waiting CDR Import";
            //    return false;
            //}

            return true;
        }
    }
}