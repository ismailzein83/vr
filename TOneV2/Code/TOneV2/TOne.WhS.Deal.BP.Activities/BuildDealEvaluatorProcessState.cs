using System;
using System.Activities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.BP.Activities
{
    public class BuildDealEvaluatorProcessState : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<DealEvaluatorProcessState> DealEvaluatorProcessState { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Byte[] maxTimestamp = new DealDetailedProgressManager().GetMaxTimestamp();
            DealEvaluatorProcessState dealEvaluatorProcessState = new DealEvaluatorProcessState() { MaxTimestamp = maxTimestamp };

            this.DealEvaluatorProcessState.Set(context, dealEvaluatorProcessState);
        }
    }
}
