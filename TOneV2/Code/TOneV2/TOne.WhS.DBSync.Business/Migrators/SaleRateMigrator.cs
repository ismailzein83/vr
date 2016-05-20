using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SaleRateMigrator : Migrator<SourceRate, SaleRate>
    {
        SaleRateDBSyncDataManager dbSyncDataManager;
        SourceRateDataManager dataManager;

        public SaleRateMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SaleRateDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceRateDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<SaleRate> itemsToAdd)
        {
            dbSyncDataManager.ApplySaleRatesToTemp(itemsToAdd);
            DBTable dbTableSaleRate = Context.DBTables[DBTableName.SaleRate];
            if (dbTableSaleRate != null)
                dbTableSaleRate.Records = dbSyncDataManager.GetSaleRates();
        }

        public override IEnumerable<SourceRate> GetSourceItems()
        {
            return dataManager.GetSourceRates(true);
        }

        public override SaleRate BuildItemFromSource(SourceRate sourceItem)
        {
            DBTable dbTableCurrency = Context.DBTables[DBTableName.Currency];
            if (dbTableCurrency != null)
            {
                Dictionary<string, Currency> allCurrencies = (Dictionary<string, Currency>)dbTableCurrency.Records;
                Currency currency = null;
                if (allCurrencies != null)
                    allCurrencies.TryGetValue(sourceItem.CurrencyId, out currency);
                if (currency != null && sourceItem.BeginEffectiveDate.HasValue && sourceItem.Rate.HasValue)
                    return new SaleRate
                    {
                        BED = sourceItem.BeginEffectiveDate.Value,
                        EED = sourceItem.EndEffectiveDate,
                        NormalRate = sourceItem.Rate.Value,
                        CurrencyId = currency.CurrencyId,
                        //PriceListId=sourceItem.PriceListId
                        //OtherRates=sourceItem.o
                        
                        //ZoneId
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
