using System;
using System.Activities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.BP.Activities
{
    public sealed class CalculateBeginDate : CodeActivity
    {
        public InArgument<DealEvaluatorProcessState> DealEvaluatorProcessState { get; set; }

        public OutArgument<DateTime?> BeginDate { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            DealEvaluatorProcessState dealEvaluatorProcessState = this.DealEvaluatorProcessState.Get(context);
            byte[] lastTimestamp = dealEvaluatorProcessState != null ? dealEvaluatorProcessState.MaxTimestamp : null;

            DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
            DateTime? beginDate = dealDetailedProgressManager.GetDealEvaluatorBeginDate(lastTimestamp);

            this.BeginDate.Set(context, beginDate);
        }
    }
}
