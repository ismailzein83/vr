using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SalePriceListMigrator : Migrator<SourcePriceList, SalePriceList>
    {
        SalePriceListDBSyncDataManager dbSyncDataManager;
        SourcePriceListDataManager dataManager;
        DBTable dbTableCurrency;
        DBTable dbTableCarrierAccount;

        public SalePriceListMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SalePriceListDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourcePriceListDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            dbTableCurrency = Context.DBTables[DBTableName.Currency];
            dbTableCarrierAccount = Context.DBTables[DBTableName.CarrierAccount];
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<SalePriceList> itemsToAdd)
        {
            dbSyncDataManager.ApplySalePriceListsToTemp(itemsToAdd);
            DBTable dbTableSalePriceList = Context.DBTables[DBTableName.SalePriceList];
            if (dbTableSalePriceList != null)
                dbTableSalePriceList.Records = dbSyncDataManager.GetSalePriceLists();
        }

        public override IEnumerable<SourcePriceList> GetSourceItems()
        {
            return dataManager.GetSourcePriceLists(true);
        }

        public override SalePriceList BuildItemFromSource(SourcePriceList sourceItem)
        {
            if (dbTableCurrency != null && dbTableCarrierAccount != null)
            {
                var allCurrencies = (Dictionary<string, Currency>)dbTableCurrency.Records;
                Currency currency = null;
                if (allCurrencies != null)
                    allCurrencies.TryGetValue(sourceItem.CurrencyId, out currency);

                var allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;
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
            else
                return null;
        }

    }
}
