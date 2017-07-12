using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Reprocess.BP.Arguments;
using System.Linq;
using Vanrise.Reprocess.Business;

namespace TOne.WhS.Deal.BP.Activities
{
    public sealed class CalculateBeginDate : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DealEvaluatorProcessState> DealEvaluatorProcessState { get; set; }

        [RequiredArgument]
        public OutArgument<DateTime?> BeginDate { get; set; }

        [RequiredArgument]
        public OutArgument<long?> LastBPInstanceId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DealEvaluatorProcessState dealEvaluatorProcessState = this.DealEvaluatorProcessState.Get(context);
            long? lastBPInstanceId = dealEvaluatorProcessState.LastBPInstanceId;

            //Loading ReprocessMinFromTime
            DateTime? reprocessMinFromTime = null;

            DealTechnicalSettingData dealTechnicalSettingData = new ConfigManager().GetDealTechnicalSettingData();
            ReprocessDefinitionManager reprocessDefinitionManager = new Vanrise.Reprocess.Business.ReprocessDefinitionManager();
            var dealReprocessDefinition = reprocessDefinitionManager.GetReprocessDefinition(dealTechnicalSettingData.ReprocessDefinitionId);

            dealReprocessDefinition.ThrowIfNull("dealReprocessDefinition", dealTechnicalSettingData.ReprocessDefinitionId);
            dealReprocessDefinition.Settings.ThrowIfNull("dealReprocessDefinition.Settings", dealTechnicalSettingData.ReprocessDefinitionId);

            List<BPInstance> bpInstances = new BPInstanceManager().GetAfterId(lastBPInstanceId, ReProcessingProcessInput.BPDefinitionId);
            if (bpInstances != null && bpInstances.Count > 0)
            {
                foreach (var bpInstance in bpInstances)
                {
                    var reProcessingSubProcessInput = bpInstance.InputArgument.CastWithValidate<ReProcessingProcessInput>("bpInstance.InputArgument", bpInstance.ProcessInstanceID);

                    var bpInstanceReprocessDefinition = reprocessDefinitionManager.GetReprocessDefinition(reProcessingSubProcessInput.ReprocessDefinitionId);
                    if (bpInstanceReprocessDefinition == null || bpInstanceReprocessDefinition.Settings == null 
                        || bpInstanceReprocessDefinition.Settings.ExecutionFlowDefinitionId != dealReprocessDefinition.Settings.ExecutionFlowDefinitionId)
                        continue;

                    if (!reprocessMinFromTime.HasValue || reprocessMinFromTime.Value > reProcessingSubProcessInput.FromTime)
                        reprocessMinFromTime = reProcessingSubProcessInput.FromTime;
                }
                lastBPInstanceId = bpInstances.Select(itm => itm.ProcessInstanceID).Max();
            }

            //Loading DealDetailedProgressBeginDate
            byte[] lastTimestamp = dealEvaluatorProcessState != null ? dealEvaluatorProcessState.DealDetailedProgressMaxTimestamp : null;
            DateTime? dealDetailedProgressBeginDate = new DealDetailedProgressManager().GetDealEvaluatorBeginDate(lastTimestamp);

            //Computing BeginDate
            DateTime? beginDate = null;

            if (!reprocessMinFromTime.HasValue)
                beginDate = dealDetailedProgressBeginDate;

            else if (!dealDetailedProgressBeginDate.HasValue)
                beginDate = reprocessMinFromTime;

            else
                beginDate = DateTime.Compare(reprocessMinFromTime.Value, dealDetailedProgressBeginDate.Value) < 0 ? reprocessMinFromTime : dealDetailedProgressBeginDate;

            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Calculate Begin Date is done", null);

            this.BeginDate.Set(context, beginDate);
            this.LastBPInstanceId.Set(context, lastBPInstanceId);
        }
    }
}
