using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Reprocess.BP.Arguments;
using Vanrise.Reprocess.Business;

namespace TOne.WhS.Deal.BP.Activities
{
	public sealed class CalculateBeginDate : CodeActivity
	{
		[RequiredArgument]
		public InArgument<DealEvaluatorProcessState> DealEvaluatorProcessState { get; set; }

		[RequiredArgument]
		public InArgument<DateTime> DealEffectiveAfter { get; set; }

		[RequiredArgument]
		public OutArgument<DateTime?> BeginDate { get; set; }

		[RequiredArgument]
		public OutArgument<long?> LastBPInstanceId { get; set; }

		protected override void Execute(CodeActivityContext context)
		{
			DealEvaluatorProcessState dealEvaluatorProcessState = this.DealEvaluatorProcessState.Get(context);
			long? lastBPInstanceId = dealEvaluatorProcessState.LastBPInstanceId;
			DateTime dealEffectiveAfter = DealEffectiveAfter.Get(context);

			//Loading ReprocessMinFromTime
			DateTime? reprocessMinFromTime = null;

			DealTechnicalSettingData dealTechnicalSettingData = new ConfigManager().GetDealTechnicalSettingData();
			ReprocessDefinitionManager reprocessDefinitionManager = new Vanrise.Reprocess.Business.ReprocessDefinitionManager();
			var dealEvaluatorReprocessDefinition = reprocessDefinitionManager.GetReprocessDefinition(dealTechnicalSettingData.ReprocessDefinitionId);
			dealEvaluatorReprocessDefinition.ThrowIfNull("dealEvaluatorReprocessDefinition", dealTechnicalSettingData.ReprocessDefinitionId);
			dealEvaluatorReprocessDefinition.Settings.ThrowIfNull("dealEvaluatorReprocessDefinition.Settings", dealTechnicalSettingData.ReprocessDefinitionId);

			List<BPInstance> bpInstances = new BPInstanceManager().GetAfterId(lastBPInstanceId, ReProcessingProcessInput.BPDefinitionId);
			if (bpInstances != null && bpInstances.Count > 0)
			{
				foreach (var bpInstance in bpInstances)
				{
					var reProcessingProcessInput = bpInstance.InputArgument.CastWithValidate<ReProcessingProcessInput>("bpInstance.InputArgument", bpInstance.ProcessInstanceID);

					var bpInstanceReprocessDefinition = reprocessDefinitionManager.GetReprocessDefinition(reProcessingProcessInput.ReprocessDefinitionId);
					if (bpInstanceReprocessDefinition == null || bpInstanceReprocessDefinition.Settings == null ||
						bpInstanceReprocessDefinition.Settings.ExecutionFlowDefinitionId != dealEvaluatorReprocessDefinition.Settings.ExecutionFlowDefinitionId)
						continue;

					if (!reprocessMinFromTime.HasValue || reprocessMinFromTime.Value > reProcessingProcessInput.FromTime)
						reprocessMinFromTime = reProcessingProcessInput.FromTime;
				}
				lastBPInstanceId = bpInstances.Select(itm => itm.ProcessInstanceID).Max();
			}

			//Loading DealDetailedProgressBeginDate
			byte[] lastTimestamp = dealEvaluatorProcessState != null ? dealEvaluatorProcessState.DealDetailedProgressMaxTimestamp : null;

			DateTime? dealDetailedProgressBeginDate = new DealDetailedProgressManager().GetDealEvaluatorBeginDate(lastTimestamp);

			//Loading DealDefinitionBeginDate
			byte[] lastDealDefinitionTimestamp = dealEvaluatorProcessState != null ? dealEvaluatorProcessState.DealDefinitionMaxTimestamp : null;
			DateTime? dealDefinitionBeginDate = new DealDefinitionManager().GetDealEvaluatorBeginDate(lastDealDefinitionTimestamp, dealEffectiveAfter);

			DateTime? beginDate = GetMinimumDate(new List<DateTime?>() { reprocessMinFromTime, dealDetailedProgressBeginDate, dealDefinitionBeginDate });

			context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Calculate Begin Date is done", null);

			this.BeginDate.Set(context, beginDate);
			this.LastBPInstanceId.Set(context, lastBPInstanceId);
		}

		public DateTime? GetMinimumDate(IEnumerable<DateTime?> dateTimes)
		{
			if (dateTimes == null || !dateTimes.Any())
				return null;
			DateTime? minDateTime = null;
			foreach (var dateTime in dateTimes)
			{
				if (dateTime.VRLessThan(minDateTime))
					minDateTime = dateTime;
			}
			return minDateTime;
		}
	}
}
