﻿using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SalePriceListMigrator : Migrator<SourcePriceList, SalePriceList>
    {
        SalePriceListDBSyncDataManager dbSyncDataManager;
        SourcePriceListDataManager dataManager;
        Dictionary<string, Currency> allCurrencies;
        Dictionary<string, CarrierAccount> allCarrierAccounts;
        public SalePriceListMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SalePriceListDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourcePriceListDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableCurrency = Context.DBTables[DBTableName.Currency];
            var dbTableCarrierAccount = Context.DBTables[DBTableName.CarrierAccount];
            allCurrencies = (Dictionary<string, Currency>)dbTableCurrency.Records;
            allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<SalePriceList> itemsToAdd)
        {
            dbSyncDataManager.ApplySalePriceListsToTemp(itemsToAdd);
        }

        public override IEnumerable<SourcePriceList> GetSourceItems()
        {
            return dataManager.GetSourcePriceLists(true);
        }

        public override SalePriceList BuildItemFromSource(SourcePriceList sourceItem)
        {

            Currency currency = null;
            if (allCurrencies != null)
                allCurrencies.TryGetValue(sourceItem.CurrencyId, out currency);


            CarrierAccount carrierAccount = null;
            if (allCarrierAccounts != null)
                allCarrierAccounts.TryGetValue(sourceItem.CustomerId, out carrierAccount);


            if (currency != null && carrierAccount != null)
                return new SalePriceList
                {
                    OwnerType = SalePriceListOwnerType.Customer,
                    OwnerId = carrierAccount.CarrierAccountId,
                    CurrencyId = currency.CurrencyId,
                    SourceId = sourceItem.SourceId
                };
            else
                return null;
        }
        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableSalePriceList = Context.DBTables[DBTableName.SalePriceList];
            if (dbTableSalePriceList != null)
                dbTableSalePriceList.Records = dbSyncDataManager.GetSalePriceLists(useTempTables);
        }
    }
}
