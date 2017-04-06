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
            ReProcessingProcessInput reprocessInputArg = context.InputArgument.CastWithValidate<ReProcessingProcessInput>("reprocessInputArg");
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
                        return false;
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
