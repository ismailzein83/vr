using System.Activities;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.BP.Activities
{
    public class BuildDealEvaluatorProcessState : CodeActivity
    {
        [RequiredArgument]
        public InArgument<long?> LastBPInstanceId { get; set; }

        [RequiredArgument]
        public InArgument<object> DealDetailedProgressMaxUpdateHandle { get; set; }

        [RequiredArgument]
        public InArgument<object> DealDefinitionMaxUpdateHandle { get; set; }

        [RequiredArgument]
        public OutArgument<DealEvaluatorProcessState> DealEvaluatorProcessState { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            long? lastBPInstanceId = this.LastBPInstanceId.Get(context);
            object dealDetailedProgressMaxUpdateHandle = this.DealDetailedProgressMaxUpdateHandle.Get(context);
            object dealDefinitionMaxUpdateHandle = this.DealDefinitionMaxUpdateHandle.Get(context);

            DealEvaluatorProcessState dealEvaluatorProcessState = new DealEvaluatorProcessState() { DealDetailedProgressMaxTimestamp = dealDetailedProgressMaxUpdateHandle, LastBPInstanceId = lastBPInstanceId, DealDefinitionMaxTimestamp = dealDefinitionMaxUpdateHandle };

            this.DealEvaluatorProcessState.Set(context, dealEvaluatorProcessState);
        }
    }
}