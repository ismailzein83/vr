using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SupplierPriceListMigrator : Migrator<SourcePriceList, SupplierPriceList>
    {
        SupplierPriceListDBSyncDataManager dbSyncDataManager;
        SourcePriceListDataManager dataManager;

        public SupplierPriceListMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SupplierPriceListDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourcePriceListDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<SupplierPriceList> itemsToAdd)
        {
            long startingId;
            ReserveIDRange(itemsToAdd.Count(), out startingId);
            dbSyncDataManager.ApplySupplierPriceListsToTemp(itemsToAdd, startingId);
            DBTable dbTableSupplierPriceList = Context.DBTables[DBTableName.SupplierPriceList];
            if (dbTableSupplierPriceList != null)
                dbTableSupplierPriceList.Records = dbSyncDataManager.GetSupplierPriceLists();
        }

        public override IEnumerable<SourcePriceList> GetSourceItems()
        {
            return dataManager.GetSourcePriceLists(false);
        }

        public override SupplierPriceList BuildItemFromSource(SourcePriceList sourceItem)
        {
            VRFileManager vrFileManager = new VRFileManager();
            DBTable dbTableCurrency = Context.DBTables[DBTableName.Currency];
            DBTable dbTableCarrierAccount = Context.DBTables[DBTableName.CarrierAccount];
            if (dbTableCurrency != null && dbTableCarrierAccount != null)
            {
                Dictionary<string, Currency> allCurrencies = (Dictionary<string, Currency>)dbTableCurrency.Records;
                Currency currency = null;
                if (allCurrencies != null)
                    allCurrencies.TryGetValue(sourceItem.CurrencyId, out currency);

                Dictionary<string, CarrierAccount> allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;
                CarrierAccount carrierAccount = null;
                if (allCarrierAccounts != null)
                    allCarrierAccounts.TryGetValue(sourceItem.SupplierId, out carrierAccount);

                long? fileId = null;

                if (sourceItem.SourceFileBytes != null)
                {
                    string[] nameastab = sourceItem.SourceFileName.Split('.');
                    VRFile file = new VRFile()
                    {
                        Content = sourceItem.SourceFileBytes,
                        Name = sourceItem.SourceFileName,
                        Extension = nameastab[nameastab.Length - 1],
                        CreatedTime = DateTime.Now

                    };

                    fileId = vrFileManager.AddFile(file);
                }

                if (currency != null && carrierAccount != null && fileId.HasValue)
                    return new SupplierPriceList
                    {
                        FileId = fileId.Value,
                        SupplierId = carrierAccount.CarrierAccountId,
                        CurrencyId = currency.CurrencyId,
                        SourceId = sourceItem.SourceId
                    };
                else
                    return null;

            }
            else
                return null;
        }

        internal static void ReserveIDRange(int nbOfIds, out long startingId)
        {
            IDManager.Instance.ReserveIDRange(typeof(SupplierPriceListManager), nbOfIds, out startingId);
        }
    }
}
