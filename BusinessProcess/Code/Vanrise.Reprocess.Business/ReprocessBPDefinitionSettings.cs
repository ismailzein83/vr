using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Reprocess.BP.Arguments;
using Vanrise.Common;

namespace Vanrise.Reprocess.Business
{
    public class ReprocessBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            ReProcessingProcessInput reprocessInputArg = context.IntanceToRun.InputArgument.CastWithValidate<ReProcessingProcessInput>("context.IntanceToRun.InputArgument");
            Dictionary<Guid, Guid> execFlowDefIdsByReprocessDefId = new Dictionary<Guid, Guid>();
            Guid executionFlowDefinitionId = GetExecFlowDefByReprocessDefId(reprocessInputArg.ReprocessDefinitionId, execFlowDefIdsByReprocessDefId);
            foreach(var startedBPInstance in context.GetStartedBPInstances())
            {
                ReProcessingProcessInput startedBPInstanceReprocessArg = startedBPInstance.InputArgument as ReProcessingProcessInput;
                if(startedBPInstanceReprocessArg != null)
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
