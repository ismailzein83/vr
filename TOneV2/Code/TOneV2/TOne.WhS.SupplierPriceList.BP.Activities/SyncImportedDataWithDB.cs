﻿using System;
using Vanrise.Entities;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Common.Business;

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
        public InArgument<TOne.WhS.BusinessEntity.Entities.SupplierPricelistType> SupplierPricelistType { get; set; }

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
            TOne.WhS.BusinessEntity.Entities.SupplierPricelistType supplierPricelistType = this.SupplierPricelistType.Get(context);
            long fileId = this.FileId.Get(context);
            long stateBackupId = this.StateBackupId.Get(context);
            DateTime effectiveOn = this.EffectiveOn.Get(context);
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            int userId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId;

            VRFileManager fileManager = new VRFileManager();
            var fileSettings = new VRFileSettings { ExtendedSettings = new TOne.WhS.BusinessEntity.Business.PriceListFileSettings { PriceListId = priceListId } };

            if (!fileManager.SetFileUsedAndUpdateSettings(fileId, fileSettings))
                throw new VRBusinessException("Pricelist files have been removed, Process must be restarted.");

            TOne.WhS.SupplierPriceList.Business.SupplierPriceListManager manager = new Business.SupplierPriceListManager();
            manager.AddPriceListAndSyncImportedDataWithDB(priceListId, processInstanceId, stateBackupId, supplierId, currencyId, fileId, effectiveOn, userId, supplierPricelistType);
        }
    }
}
