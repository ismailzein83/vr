using System;
using System.Linq;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.BP.Activities
{
    public sealed class PrepareRecurChargeBalanceUpdateSummary : CodeActivity
    {
        [RequiredArgument]
        public InArgument<List<AccountPackageRecurCharge>> AccountPackageRecurChargeList { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> ChargeDay { get; set; }

        [RequiredArgument]
        public OutArgument<RecurChargeBalanceUpdateSummary> RecurChargeBalanceUpdateSummary { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            List<AccountPackageRecurCharge> accountPackageRecurChargeList = this.AccountPackageRecurChargeList.Get(context);
            RecurChargeBalanceUpdateSummary recurChargeBalanceUpdateSummary = null;

            if (accountPackageRecurChargeList != null)
            {
                recurChargeBalanceUpdateSummary = new Entities.RecurChargeBalanceUpdateSummary()
                {
                    ChargeDay = this.ChargeDay.Get(context),
                    AccountPackageRecurChargeKeys = accountPackageRecurChargeList.Select(itm => new AccountPackageRecurChargeKey() 
                    { 
                        BalanceAccountTypeID = itm.BalanceAccountTypeID, 
                        ChargeDay = itm.ChargeDay, 
                        TransactionTypeId = itm.TransactionTypeID 
                    }).ToHashSet()
                };
            }

            this.RecurChargeBalanceUpdateSummary.Set(context, recurChargeBalanceUpdateSummary);
        }
    }
}