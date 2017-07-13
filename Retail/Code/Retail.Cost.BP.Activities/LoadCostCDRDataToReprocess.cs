using System;
using System.Activities;
using System.Collections.Generic;
using Retail.Cost.Entities;
using Retail.Cost.Business;

namespace Retail.Cost.BP.Activities
{
    public sealed class LoadCostCDRDataToReprocess : CodeActivity
    {
        [RequiredArgument]
        public InOutArgument<CDRCostEvaluatorProcessState> CDRCostEvaluatorProcessState { get; set; }

        [RequiredArgument]
        public OutArgument<HashSet<DateTime>> DaysToReprocess { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            CDRCostEvaluatorProcessState cdrCostEvaluatorProcessState = context.GetValue(this.CDRCostEvaluatorProcessState);
            long? lastCDRCostProcessedId = cdrCostEvaluatorProcessState != null ? cdrCostEvaluatorProcessState.LastCDRCostProcessedId : null;

            CDRCostManager cdrCostManager = new CDRCostManager();
            long? maxCDRCostId = cdrCostManager.GetMaxCDRCostId();
            HashSet<DateTime> daysToReprocess = cdrCostManager.GetDistinctDatesAfterId(lastCDRCostProcessedId);

            if (cdrCostEvaluatorProcessState == null)
                cdrCostEvaluatorProcessState = new CDRCostEvaluatorProcessState();

            cdrCostEvaluatorProcessState.LastCDRCostProcessedId = maxCDRCostId;

            this.CDRCostEvaluatorProcessState.Set(context, cdrCostEvaluatorProcessState);
            this.DaysToReprocess.Set(context, daysToReprocess);
        }
    }
}