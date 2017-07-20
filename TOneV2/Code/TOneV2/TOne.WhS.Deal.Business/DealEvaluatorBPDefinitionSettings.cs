using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;
using Vanrise.Reprocess.Business;
using Vanrise.Reprocess.Entities;

namespace TOne.WhS.Deal.Business
{
    public class DealEvaluatorBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings 
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            ConfigManager configManager = new ConfigManager();
            var dealTechnicalSettingData = configManager.GetDealTechnicalSettingData();
            Guid dealEvaluatorReprocessDefinitionId = dealTechnicalSettingData.ReprocessDefinitionId;
            
            HoldRequestManager holdRequestManager = new HoldRequestManager();
            DateTimeRange dateTimeRange = holdRequestManager.GetDBDateTimeRange();

            Dictionary<Guid, Guid> execFlowDefIdsByReprocessDefId = new Dictionary<Guid, Guid>();
            Guid executionFlowDefinitionId = GetExecFlowDefByReprocessDefId(dealEvaluatorReprocessDefinitionId, execFlowDefIdsByReprocessDefId);

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                var startedBPInstanceReprocessArg = startedBPInstance.InputArgument as Vanrise.Reprocess.BP.Arguments.ReProcessingProcessInput;
                if (startedBPInstanceReprocessArg != null)
                {
                    Guid startedBPInstanceExecFlowDefId = GetExecFlowDefByReprocessDefId(startedBPInstanceReprocessArg.ReprocessDefinitionId, execFlowDefIdsByReprocessDefId);
                    if (startedBPInstanceExecFlowDefId == executionFlowDefinitionId
                        && Utilities.AreTimePeriodsOverlapped(dateTimeRange.From, dateTimeRange.To, startedBPInstanceReprocessArg.FromTime, startedBPInstanceReprocessArg.ToTime))
                    {
                        context.Reason = String.Format("Another reprocess instance is running from {0:yyyy-MM-dd} to {1:yyyy-MM-dd}", startedBPInstanceReprocessArg.FromTime, startedBPInstanceReprocessArg.ToTime);
                        return false;
                    }
                }
            }

            Vanrise.Queueing.Entities.HoldRequest existingHoldRequest = holdRequestManager.GetHoldRequest(context.IntanceToRun.ProcessInstanceID);
            if (existingHoldRequest == null)
            {
                ReprocessDefinitionManager reprocessDefinitionManager = new ReprocessDefinitionManager();
                ReprocessDefinition reprocessDefinition = reprocessDefinitionManager.GetReprocessDefinition(dealEvaluatorReprocessDefinitionId);

                holdRequestManager.InsertHoldRequest(context.IntanceToRun.ProcessInstanceID, reprocessDefinition.Settings.ExecutionFlowDefinitionId, dateTimeRange.From, dateTimeRange.To,
                    reprocessDefinition.Settings.StagesToHoldNames, reprocessDefinition.Settings.StagesToProcessNames, HoldRequestStatus.Pending);

                context.Reason = "Waiting CDR Import";
                return false;
            }
            else if (existingHoldRequest.Status != HoldRequestStatus.CanBeStarted)
            {
                context.Reason = "Waiting CDR Import";
                return false;
            }

            return true;
        }

        static Vanrise.Reprocess.Business.ReprocessDefinitionManager s_reprocessDefinitionManager = new Vanrise.Reprocess.Business.ReprocessDefinitionManager();
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
