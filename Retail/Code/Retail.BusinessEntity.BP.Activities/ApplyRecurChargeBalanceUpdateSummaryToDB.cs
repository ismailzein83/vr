using System;
using System.Activities;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Business;

namespace Retail.BusinessEntity.BP.Activities
{
    public sealed class ApplyRecurChargeBalanceUpdateSummaryToDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<RecurChargeBalanceUpdateSummary> RecurChargeBalanceUpdateSummary { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            RecurChargeBalanceUpdateSummaryManager RecurChargeBalanceUpdateSummaryManager = new RecurChargeBalanceUpdateSummaryManager();
            RecurChargeBalanceUpdateSummaryManager.Insert(this.RecurChargeBalanceUpdateSummary.Get(context));
        }
    }
}