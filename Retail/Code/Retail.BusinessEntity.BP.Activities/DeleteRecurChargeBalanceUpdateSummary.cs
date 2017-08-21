using System;
using System.Activities;
using Retail.BusinessEntity.Business;

namespace Retail.BusinessEntity.BP.Activities
{
    public sealed class DeleteRecurChargeBalanceUpdateSummary : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime > ChargeDay { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            RecurChargeBalanceUpdateSummaryManager RecurChargeBalanceUpdateSummaryManager = new RecurChargeBalanceUpdateSummaryManager();
            RecurChargeBalanceUpdateSummaryManager.Delete(this.ChargeDay.Get(context));
        }
    }
}