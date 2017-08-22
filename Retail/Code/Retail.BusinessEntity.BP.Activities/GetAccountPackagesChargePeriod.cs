using System;
using System.Linq;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.BusinessProcess;
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
        public OutArgument<DateTime?> MaximumEndChargePeriod { get; set; }

        [RequiredArgument]
        public OutArgument<List<AccountPackageRecurChargeData>> AccountPackageRecurChargeDataList { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Loading Account Packages Charging Period has started", null);
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

            DateTime maximumEndChargePeriod = accountPackageRecurChargeDataList.Select(itm => itm.EndChargePeriod).Max().AddDays(-1);

            this.MaximumEndChargePeriod.Set(context, maximumEndChargePeriod);
            this.AccountPackageRecurChargeDataList.Set(context, accountPackageRecurChargeDataList);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Loading Account Packages Charging Period is done", null);
        }

        private DateTime GetEndChargePeriod(DateTime effectiveDate)
        {
            DateTime modifiedDate = DateTime.Now.AddMonths(1);
            return new DateTime(modifiedDate.Year, modifiedDate.Month, 1);
        }
    }
}
