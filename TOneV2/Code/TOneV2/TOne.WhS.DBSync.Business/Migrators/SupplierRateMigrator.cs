using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using System.Linq;

namespace TOne.WhS.DBSync.Business
{
    public class SupplierRateMigrator : Migrator<SourceRate, SupplierRate>
    {
        SupplierRateDBSyncDataManager dbSyncDataManager;
        SourceRateDataManager dataManager;

        public SupplierRateMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SupplierRateDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceRateDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<SupplierRate> itemsToAdd)
        {
            long startingId;
            ReserveIDRange(itemsToAdd.Count(), out startingId);
            dbSyncDataManager.ApplySupplierRatesToTemp(itemsToAdd, startingId);
            DBTable dbTableSupplierRate = Context.DBTables[DBTableName.SupplierRate];
            if (dbTableSupplierRate != null)
                dbTableSupplierRate.Records = dbSyncDataManager.GetSupplierRates();
        }

        public override IEnumerable<SourceRate> GetSourceItems()
        {
            return dataManager.GetSourceRates(false);
        }

        public override SupplierRate BuildItemFromSource(SourceRate sourceItem)
        {
            DBTable dbTableSupplierZone = Context.DBTables[DBTableName.SupplierZone];
            DBTable dbTableSupplierPriceList = Context.DBTables[DBTableName.SupplierPriceList];
            DBTable dbTableCurrency = Context.DBTables[DBTableName.Currency];
            if (dbTableCurrency != null && dbTableSupplierPriceList != null && dbTableSupplierZone != null)
            {
                Dictionary<string, SupplierZone> allSupplierZones = (Dictionary<string, SupplierZone>)dbTableSupplierZone.Records;
                Dictionary<string, SupplierPriceList> allSupplierPriceLists = (Dictionary<string, SupplierPriceList>)dbTableSupplierPriceList.Records;
                Dictionary<string, Currency> allCurrencies = (Dictionary<string, Currency>)dbTableCurrency.Records;
                SupplierZone saleZone = null;
                SupplierPriceList salePriceList = null;
                Currency currency = null;
                if (allCurrencies != null)
                    allCurrencies.TryGetValue(sourceItem.CurrencyId, out currency);
                if (allSupplierPriceLists != null && sourceItem.PriceListId.HasValue)
                    allSupplierPriceLists.TryGetValue(sourceItem.PriceListId.Value.ToString(), out salePriceList);

                if (allSupplierZones != null && sourceItem.ZoneId.HasValue)
                    allSupplierZones.TryGetValue(sourceItem.ZoneId.Value.ToString(), out saleZone);


                Dictionary<int, decimal> otherRates = new Dictionary<int, decimal>();
                if (sourceItem.OffPeakRate.HasValue)
                    otherRates.Add((int)RateTypeEnum.OffPeak, sourceItem.OffPeakRate.Value);

                if (sourceItem.WeekendRate.HasValue)
                    otherRates.Add((int)RateTypeEnum.Weekend, sourceItem.WeekendRate.Value);

                if (salePriceList != null && currency != null && sourceItem.BeginEffectiveDate.HasValue && sourceItem.Rate.HasValue)
                    return new SupplierRate
                    {
                        BED = sourceItem.BeginEffectiveDate.Value,
                        EED = sourceItem.EndEffectiveDate,
                        NormalRate = sourceItem.Rate.Value,
                        CurrencyId = currency.CurrencyId,
                        PriceListId = salePriceList.PriceListId,
                        OtherRates = otherRates,
                        ZoneId = saleZone.SupplierZoneId,
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
            IDManager.Instance.ReserveIDRange(typeof(SaleRateManager), nbOfIds, out startingId);
        }
    }
}
