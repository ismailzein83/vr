using System;
using System.Linq;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Business;

namespace Retail.BusinessEntity.BP.Activities
{
    public sealed class ApplyReccuringChargeToDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<AccountPackageRecurChargeKey, List<AccountPackageRecurCharge>>> AccountPackageRecurChargeDict { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Dictionary<AccountPackageRecurChargeKey, List<AccountPackageRecurCharge>> accountPackageRecurChargeDict = context.GetValue(this.AccountPackageRecurChargeDict);
            DateTime effectiveDate = context.GetValue(this.EffectiveDate);
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;

            List<AccountPackageRecurCharge> accountPackageRecurChargeList = accountPackageRecurChargeDict.Values.SelectMany(itm => itm).ToList();
            AccountPackageRecurChargeManager accountPackageRecurChargeManager = new AccountPackageRecurChargeManager();
            accountPackageRecurChargeManager.ApplyAccountPackageReccuringCharges(accountPackageRecurChargeList, effectiveDate, processInstanceId);
        }
    }
}