using System;
using System.Activities;
using System.Collections.Generic;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Business;

namespace Retail.BusinessEntity.BP.Activities
{
    public sealed class LoadAccountPackageRecurChargeKeysFromDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> ChargeDay { get; set; }

        [RequiredArgument]
        public OutArgument<HashSet<AccountPackageRecurChargeKey>> AccountPackageRecurChargeKeys { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime chargeDay = this.ChargeDay.Get(context);
            AccountPackageRecurChargeManager accountPackageRecurChargeManager = new AccountPackageRecurChargeManager();
            HashSet<AccountPackageRecurChargeKey> accountPackageRecurChargeKeys = accountPackageRecurChargeManager.GetAccountRecurringChargeKeys(chargeDay);

            this.AccountPackageRecurChargeKeys.Set(context, accountPackageRecurChargeKeys);
        }
    }
}