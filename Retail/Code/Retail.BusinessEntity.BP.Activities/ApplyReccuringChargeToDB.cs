using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Business;
using Vanrise.Entities;

namespace Retail.BusinessEntity.BP.Activities
{
    public sealed class ApplyReccuringChargeToDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<List<AccountPackageRecurCharge>> AccountPackageRecurChargeList { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Apply Reccuring Charge To Database has started", null);
            List<AccountPackageRecurCharge> accountPackageRecurChargeList = context.GetValue(this.AccountPackageRecurChargeList);
            DateTime effectiveDate = context.GetValue(this.EffectiveDate);
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;

            AccountPackageRecurChargeManager accountPackageRecurChargeManager = new AccountPackageRecurChargeManager();
            accountPackageRecurChargeManager.ApplyAccountPackageReccuringCharges(accountPackageRecurChargeList, effectiveDate, processInstanceId);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Apply Reccuring Charge To Database is done", null);
        }
    }
}