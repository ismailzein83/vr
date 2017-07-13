using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Retail.Cost.Entities;
using Retail.Cost.Business;

namespace Retail.Cost.BP.Activities
{
    public sealed class UpadeOverridenCostCDR : CodeActivity
    {
        [RequiredArgument]
        public InArgument<CDRCostEvaluatorProcessState> CDRCostEvaluatorProcessState { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            CDRCostEvaluatorProcessState cdrCostEvaluatorProcessState = context.GetValue(this.CDRCostEvaluatorProcessState);
            long? lastCDRCostProcessedId = cdrCostEvaluatorProcessState != null ? cdrCostEvaluatorProcessState.LastCDRCostProcessedId : null;

            CDRCostManager cdrCostManager = new CDRCostManager();
            cdrCostManager.UpadeOverridenCostCDRAfterId(lastCDRCostProcessedId);
        }
    }
}
