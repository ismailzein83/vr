using System;
using System.Collections.Generic;
using Vanrise.Reprocess.BP.Arguments;
using Vanrise.Common;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;
using Vanrise.Reprocess.Entities;

namespace Vanrise.Reprocess.Business
{
    public class ReprocessBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override void OnBPExecutionCompleted(BusinessProcess.Entities.IBPDefinitionBPExecutionCompletedContext context)
        {
            HoldRequestManager holdRequestManager = new HoldRequestManager();
            holdRequestManager.DeleteHoldRequestByBPInstanceId(context.BPInstance.ProcessInstanceID);
        }
        
        public override bool CanRunBPInstance(BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            ReProcessingProcessInput reprocessInputArg = context.IntanceToRun.InputArgument.CastWithValidate<ReProcessingProcessInput>("context.IntanceToRun.InputArgument");

            if (!reprocessInputArg.IgnoreSynchronisation)
            {
                Dictionary<Guid, Guid> execFlowDefIdsByReprocessDefId = new Dictionary<Guid, Guid>();
                Guid executionFlowDefinitionId = GetExecFlowDefByReprocessDefId(reprocessInputArg.ReprocessDefinitionId, execFlowDefIdsByReprocessDefId);
                foreach (var startedBPInstance in context.GetStartedBPInstances())
                {
                    ReProcessingProcessInput startedBPInstanceReprocessArg = startedBPInstance.InputArgument as ReProcessingProcessInput;
                    if (startedBPInstanceReprocessArg != null)
                    {
                        Guid startedBPInstanceExecFlowDefId = GetExecFlowDefByReprocessDefId(startedBPInstanceReprocessArg.ReprocessDefinitionId, execFlowDefIdsByReprocessDefId);
                        if (startedBPInstanceExecFlowDefId == executionFlowDefinitionId
                            && Utilities.AreTimePeriodsOverlapped(reprocessInputArg.FromTime, reprocessInputArg.ToTime, startedBPInstanceReprocessArg.FromTime, startedBPInstanceReprocessArg.ToTime))
                        {
                            context.Reason = String.Format("Another reprocess instance is running from {0:yyyy-MM-dd} to {1:yyyy-MM-dd}", startedBPInstanceReprocessArg.FromTime, startedBPInstanceReprocessArg.ToTime);
                            return false;
                        }
                    }
                }

                HoldRequestManager holdRequestManager = new HoldRequestManager();
                HoldRequest existingHoldRequest = holdRequestManager.GetHoldRequest(context.IntanceToRun.ProcessInstanceID);
                if (existingHoldRequest == null)
                {
                    ReprocessDefinitionManager reprocessDefinitionManager = new ReprocessDefinitionManager();
                    ReprocessDefinition reprocessDefinition = reprocessDefinitionManager.GetReprocessDefinition(reprocessInputArg.ReprocessDefinitionId);

                    holdRequestManager.InsertHoldRequest(context.IntanceToRun.ProcessInstanceID, reprocessDefinition.Settings.ExecutionFlowDefinitionId, reprocessInputArg.FromTime, reprocessInputArg.ToTime,
                        reprocessDefinition.Settings.StagesToHoldNames, reprocessDefinition.Settings.StagesToProcessNames, HoldRequestStatus.Pending);

                    context.Reason = "Waiting CDR Import";
                    return false;
                }
                else if (existingHoldRequest.Status != HoldRequestStatus.CanBeStarted)
                {
                    context.Reason = "Waiting CDR Import";
                    return false;
                }
            }

            return true;
        }

        public override bool CanCancelBPInstance(BusinessProcess.Entities.IBPDefinitionCanCancelBPInstanceContext context)
        {
            return true;
        }

        static ReprocessDefinitionManager s_reprocessDefinitionManager = new ReprocessDefinitionManager();
        private Guid GetExecFlowDefByReprocessDefId(Guid reprocessDefinitionId, Dictionary<Guid, Guid> execFlowDefIdsByReprocessDefId)
        {
            Guid execFlowDefId;
            if (execFlowDefIdsByReprocessDefId.TryGetValue(reprocessDefinitionId, out execFlowDefId))
            {
                return execFlowDefId;
            }
            else
            {
                var reprocessDefinition = s_reprocessDefinitionManager.GetReprocessDefinition(reprocessDefinitionId);
                reprocessDefinition.ThrowIfNull("reprocessDefinition", reprocessDefinitionId);
                reprocessDefinition.Settings.ThrowIfNull("reprocessDefinition.Settings", reprocessDefinitionId);
                execFlowDefId = reprocessDefinition.Settings.ExecutionFlowDefinitionId;
                execFlowDefIdsByReprocessDefId.Add(reprocessDefinitionId, execFlowDefId);
                return execFlowDefId;
            }
        }
    }
}
