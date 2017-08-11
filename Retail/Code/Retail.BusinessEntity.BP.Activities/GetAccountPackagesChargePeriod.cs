using System;
using System.Collections.Generic;
using System.Activities;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.BP.Activities
{
    public sealed class GetAccountPackagesChargePeriod : CodeActivity
    {
        [RequiredArgument]
        public InArgument<List<AccountPackage>> AccountPackages { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public OutArgument<List<AccountPackageRecurChargeData>> AccountPackageRecurChargeDataList { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<AccountPackage> accountPackages = context.GetValue(this.AccountPackages);
            DateTime effectiveDate = context.GetValue(this.EffectiveDate);

            List<AccountPackageRecurChargeData> accountPackageRecurChargeDataList = new List<AccountPackageRecurChargeData>();
            foreach (AccountPackage accountPackage in accountPackages)
            {
                AccountPackageRecurChargeData accountPackageRecurChargeData = new AccountPackageRecurChargeData()
                {
                    AccountPackage = accountPackage,
                    BeginChargePeriod = effectiveDate,
                    EndChargePeriod = GetEndChargePeriod(effectiveDate)
                };
                accountPackageRecurChargeDataList.Add(accountPackageRecurChargeData);
            }

            this.AccountPackageRecurChargeDataList.Set(context, accountPackageRecurChargeDataList);
        }

        private DateTime GetEndChargePeriod(DateTime effectiveDate)
        {
            effectiveDate = effectiveDate.AddMonths(1);
            return new DateTime(effectiveDate.Year, effectiveDate.Month, 1);
        }
    }
}
