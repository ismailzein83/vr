using System;
using System.Activities;
using System.Collections.Generic;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Business;

namespace Retail.BusinessEntity.BP.Activities
{
    public sealed class LoadRecurChargeBalanceUpdateSummaryListFromDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> ChargeDay { get; set; }

        [RequiredArgument]
        public OutArgument<List<RecurChargeBalanceUpdateSummary>> RecurChargeBalanceUpdateSummaryList { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime chargeDay = this.ChargeDay.Get(context);
            List<RecurChargeBalanceUpdateSummary> recurChargeBalanceUpdateSummaryList = new RecurChargeBalanceUpdateSummaryManager().GetRecurChargeBalanceUpdateSummaryList(chargeDay);

            this.RecurChargeBalanceUpdateSummaryList.Set(context, recurChargeBalanceUpdateSummaryList);
        }
    }
}