using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.BP.Activities
{
    public sealed class GetAccountPackageRecurChargeList : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> ChargeDay { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<DateTime, List<AccountPackageRecurCharge>>> AccountPackageRecurChargesByDate { get; set; }

        [RequiredArgument]
        public OutArgument<List<AccountPackageRecurCharge>> AccountPackageRecurChargeList { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Dictionary<DateTime, List<AccountPackageRecurCharge>> accountPackageRecurChargesByDate = this.AccountPackageRecurChargesByDate.Get(context);
            DateTime chargeDay = this.ChargeDay.Get(context);

            if (accountPackageRecurChargesByDate != null)
            {
                List<AccountPackageRecurCharge> accountPackageRecurChargeList = accountPackageRecurChargesByDate.GetRecord(chargeDay);
                AccountPackageRecurChargeList.Set(context, accountPackageRecurChargeList);
            }
        }
    }
}