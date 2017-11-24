using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SupplierPriceListMigrator : Migrator<SourcePriceList, SupplierPriceList>
    {
        SupplierPriceListDBSyncDataManager dbSyncDataManager;
        SourcePriceListDataManager dataManager;
        FileDBSyncDataManager fileDataManager;
        Dictionary<string, Currency> allCurrencies;
        Dictionary<string, CarrierAccount> allCarrierAccounts;
        public SupplierPriceListMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SupplierPriceListDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourcePriceListDataManager(Context.ConnectionString, Context.EffectiveAfterDate, Context.OnlyEffective);
            fileDataManager = new FileDBSyncDataManager(context.UseTempTables);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableCurrency = Context.DBTables[DBTableName.Currency];
            var dbTableCarrierAccount = Context.DBTables[DBTableName.CarrierAccount];
            allCurrencies = (Dictionary<string, Currency>)dbTableCurrency.Records;
            allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;

        }

        public override void Migrate(MigrationInfoContext context)
        {
            SupplierPriceListManager manager = new SupplierPriceListManager();
            context.GeneratedIdsInfoContext = new GeneratedIdsInfoContext();
            context.GeneratedIdsInfoContext.TypeId = manager.GetSupplierPriceListTypeId();
            base.Migrate(context);
        }

        public override void AddItems(List<SupplierPriceList> itemsToAdd)
        {
            dbSyncDataManager.ApplySupplierPriceListsToTemp(itemsToAdd);
        }

        public override IEnumerable<SourcePriceList> GetSourceItems()
        {
            return dataManager.GetSourcePriceLists(false, Context.MigratePriceListData);
        }


        public override SupplierPriceList BuildItemFromSource(SourcePriceList sourceItem)
        {
            Currency currency = null;
            if (allCurrencies != null)
                allCurrencies.TryGetValue(sourceItem.CurrencyId, out currency);

            CarrierAccount carrierAccount = null;
            if (allCarrierAccounts != null)
                allCarrierAccounts.TryGetValue(sourceItem.SupplierId, out carrierAccount);

            long? fileId = null;
            if (sourceItem.SourceFileBytes != null)
            {
                string[] nameastab = sourceItem.SourceFileName.Split('.');
                var fileSettings = new VRFileSettings { ExtendedSettings = new TOne.BusinessEntity.Business.PriceListFileSettings { PriceListId = ++TotalRowsSuccess } };

                VRFile file = new VRFile()
                {
                    Content = sourceItem.SourceFileBytes,
                    Name = sourceItem.SourceFileName,
                    Extension = nameastab[nameastab.Length - 1],
                    CreatedTime = sourceItem.BED,
                    IsTemp = false,
                    Settings = fileSettings,
                };

                fileId = fileDataManager.ApplyFile(file);
            }


            if (currency != null && carrierAccount != null)
                return new SupplierPriceList
                {
                    FileId = fileId,
                    SupplierId = carrierAccount.CarrierAccountId,
                    CurrencyId = currency.CurrencyId,
                    SourceId = sourceItem.SourceId,
                    CreateTime = sourceItem.BED,
                    EffectiveOn = sourceItem.BED,
                    PriceListId = TotalRowsSuccess,
                };
            else
            {
                TotalRowsFailed++;
                //Context.WriteWarning(string.Format("Failed migrating Supplier Pricelist, Source Id: {0}", sourceItem.SourceId));
                return null;
            }

        }

        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableSupplierPriceList = Context.DBTables[DBTableName.SupplierPriceList];
            if (dbTableSupplierPriceList != null)
                dbTableSupplierPriceList.Records = dbSyncDataManager.GetSupplierPriceLists(useTempTables);
        }

        public override void LoadSourceItems(Action<SourcePriceList> onItemLoaded)
        {
            dataManager.LoadSourceItems(false, Context.MigratePriceListData, onItemLoaded);
        }

        public override bool IsLoadItemsApproach
        {
            get
            {
                return true;
            }
        }
    }
    
}
