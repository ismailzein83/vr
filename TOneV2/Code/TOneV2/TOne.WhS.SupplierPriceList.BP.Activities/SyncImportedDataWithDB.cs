using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class SyncImportedDataWithDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> PriceListId { get; set; }

        [RequiredArgument]
        public InArgument<int> SupplierId { get; set; }

        [RequiredArgument]
        public InArgument<int> CurrencyId { get; set; }

        [RequiredArgument]
        public InArgument<long> FileId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<long> StateBackupId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int priceListId = this.PriceListId.Get(context);
            int supplierId = this.SupplierId.Get(context);
            int currencyId = this.CurrencyId.Get(context);
            long fileId = this.FileId.Get(context);
            long stateBackupId = this.StateBackupId.Get(context);
            DateTime effectiveOn = this.EffectiveOn.Get(context);
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            int userId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId;

            TOne.WhS.SupplierPriceList.Business.SupplierPriceListManager manager = new Business.SupplierPriceListManager();
            manager.AddPriceListAndSyncImportedDataWithDB(priceListId, processInstanceId, stateBackupId, supplierId, currencyId, fileId, effectiveOn, userId);
        }
    }
}
