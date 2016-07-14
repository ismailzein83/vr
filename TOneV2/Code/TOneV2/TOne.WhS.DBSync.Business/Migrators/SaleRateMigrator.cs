using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SaleRateMigrator : Migrator<SourceRate, SaleRate>
    {
        SaleRateDBSyncDataManager dbSyncDataManager;
        SourceRateDataManager dataManager;
        Dictionary<string, SaleZone> allSaleZones;
        Dictionary<string, Currency> allCurrencies;
        Dictionary<string, SalePriceList> allSalePriceLists;

        public SaleRateMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SaleRateDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceRateDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableSaleZone = Context.DBTables[DBTableName.SaleZone];
            var dbTableSalePriceList = Context.DBTables[DBTableName.SalePriceList];
            var dbTableCurrency = Context.DBTables[DBTableName.Currency];
            allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZone.Records;
            allCurrencies = (Dictionary<string, Currency>)dbTableCurrency.Records;
            allSalePriceLists = (Dictionary<string, SalePriceList>)dbTableSalePriceList.Records;
        }

        public override void Migrate(MigrationInfoContext context)
        {
            SaleRateManager manager = new SaleRateManager();
            context.GeneratedIdsInfoContext = new GeneratedIdsInfoContext();
            context.GeneratedIdsInfoContext.TypeId = manager.GetSaleRateTypeId();
            base.Migrate(context);
            
        }

        public override void AddItems(List<SaleRate> itemsToAdd)
        {
            dbSyncDataManager.ApplySaleRatesToTemp(itemsToAdd, 1);
            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceRate> GetSourceItems()
        {
            return dataManager.GetSourceRates(true);
        }

        public override SaleRate BuildItemFromSource(SourceRate sourceItem)
        {

            SaleZone saleZone = null;
            SalePriceList salePriceList = null;
            Currency currency = null;
            if (allCurrencies != null)
                allCurrencies.TryGetValue(sourceItem.CurrencyId, out currency);
            if (allSalePriceLists != null && sourceItem.PriceListId.HasValue)
                allSalePriceLists.TryGetValue(sourceItem.PriceListId.Value.ToString(), out salePriceList);

            if (allSaleZones != null && sourceItem.ZoneId.HasValue)
                allSaleZones.TryGetValue(sourceItem.ZoneId.Value.ToString(), out saleZone);


            Dictionary<int, decimal> otherRates = new Dictionary<int, decimal>();
            if (sourceItem.OffPeakRate.HasValue)
                otherRates.Add((int)RateTypeEnum.OffPeak, sourceItem.OffPeakRate.Value);

            if (sourceItem.WeekendRate.HasValue)
                otherRates.Add((int)RateTypeEnum.Weekend, sourceItem.WeekendRate.Value);

            if (salePriceList != null && currency != null && sourceItem.BeginEffectiveDate.HasValue && sourceItem.Rate.HasValue)
                return new SaleRate
                {
                    BED = sourceItem.BeginEffectiveDate.Value,
                    EED = sourceItem.EndEffectiveDate,
                    NormalRate = sourceItem.Rate.Value,
                    CurrencyId = currency.CurrencyId,
                    PriceListId = salePriceList.PriceListId,
                    OtherRates = otherRates,
                    ZoneId = saleZone.SaleZoneId,
                    SourceId = sourceItem.SourceId
                };
            else
            {
                TotalRowsFailed++;
                return null;
            }
        }
        public override void FillTableInfo(bool useTempTables)
        {
           
        }
    }
}
