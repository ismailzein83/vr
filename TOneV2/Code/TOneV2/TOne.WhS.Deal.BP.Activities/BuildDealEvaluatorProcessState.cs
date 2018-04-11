using System;
using System.Activities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.BP.Activities
{
	public class BuildDealEvaluatorProcessState : CodeActivity
	{
		[RequiredArgument]
		public InArgument<long?> LastBPInstanceId { get; set; }

		[RequiredArgument]
		public InArgument<byte[]> DealDetailedProgressMaxTimestamp { get; set; }

		[RequiredArgument]
		public InArgument<byte[]> DealDefinitionMaxTimestamp { get; set; }

		[RequiredArgument]
		public OutArgument<DealEvaluatorProcessState> DealEvaluatorProcessState { get; set; }

		protected override void Execute(CodeActivityContext context)
		{
			long? lastBPInstanceId = this.LastBPInstanceId.Get(context);
			Byte[] maxTimestamp = this.DealDetailedProgressMaxTimestamp.Get(context);
			Byte[] dealDefinitionMaxTimestamp = this.DealDefinitionMaxTimestamp.Get(context);

			DealEvaluatorProcessState dealEvaluatorProcessState = new DealEvaluatorProcessState() { DealDetailedProgressMaxTimestamp = maxTimestamp, LastBPInstanceId = lastBPInstanceId, DealDefinitionMaxTimestamp = dealDefinitionMaxTimestamp };

			this.DealEvaluatorProcessState.Set(context, dealEvaluatorProcessState);
		}
	}
}
