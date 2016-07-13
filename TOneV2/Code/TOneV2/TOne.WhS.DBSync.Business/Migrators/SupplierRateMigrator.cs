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
    public class SupplierRateMigrator : Migrator<SourceRate, SupplierRate>
    {
        SupplierRateDBSyncDataManager dbSyncDataManager;
        SourceRateDataManager dataManager;
        Dictionary<string, SupplierZone> allSupplierZones;
        Dictionary<string, SupplierPriceList> allSupplierPriceLists;
        Dictionary<string, Currency> allCurrencies;
        int _offPeakRateTypeId;
        int _weekendRateTypeId;
        public SupplierRateMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SupplierRateDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceRateDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableSupplierZone = Context.DBTables[DBTableName.SupplierZone];
            var dbTableSupplierPriceList = Context.DBTables[DBTableName.SupplierPriceList];
            var dbTableCurrency = Context.DBTables[DBTableName.Currency];
            allSupplierZones = (Dictionary<string, SupplierZone>)dbTableSupplierZone.Records;
            allSupplierPriceLists = (Dictionary<string, SupplierPriceList>)dbTableSupplierPriceList.Records;
            allCurrencies = (Dictionary<string, Currency>)dbTableCurrency.Records;
            _offPeakRateTypeId = context.OffPeakRateTypeId;
            _weekendRateTypeId = context.WeekendRateTypeId;
        }

        public override void Migrate(MigrationInfoContext context)
        {
            SupplierRateManager manager = new SupplierRateManager();
            context.GeneratedIdsInfoContext = new GeneratedIdsInfoContext();
            context.GeneratedIdsInfoContext.TypeId = manager.GetSupplierRateTypeId();
            base.Migrate(context);
        }

        public override void AddItems(List<SupplierRate> itemsToAdd)
        {
            dbSyncDataManager.ApplySupplierRatesToTemp(itemsToAdd, 1);
            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceRate> GetSourceItems()
        {
            return dataManager.GetSourceRates(false);
        }

        public override SupplierRate BuildItemFromSource(SourceRate sourceItem)
        {
            SupplierZone supplierZone = null;
            SupplierPriceList supplierPriceList = null;
            Currency currency = null;
            if (allCurrencies != null)
                allCurrencies.TryGetValue(sourceItem.CurrencyId, out currency);
            if (allSupplierPriceLists != null && sourceItem.PriceListId.HasValue)
                allSupplierPriceLists.TryGetValue(sourceItem.PriceListId.Value.ToString(), out supplierPriceList);

            if (allSupplierZones != null && sourceItem.ZoneId.HasValue)
                allSupplierZones.TryGetValue(sourceItem.ZoneId.Value.ToString(), out supplierZone);


            Dictionary<int, decimal> otherRates = new Dictionary<int, decimal>();
            if (sourceItem.OffPeakRate.HasValue)
                otherRates.Add(_offPeakRateTypeId, sourceItem.OffPeakRate.Value);

            if (sourceItem.WeekendRate.HasValue)
                otherRates.Add(_weekendRateTypeId, sourceItem.WeekendRate.Value);

            if (supplierZone != null && supplierPriceList != null && currency != null && sourceItem.BeginEffectiveDate.HasValue && sourceItem.Rate.HasValue)
                return new SupplierRate
                {
                    BED = sourceItem.BeginEffectiveDate.Value,
                    EED = sourceItem.EndEffectiveDate,
                    NormalRate = sourceItem.Rate.Value,
                    CurrencyId = currency.CurrencyId,
                    PriceListId = supplierPriceList.PriceListId,
                    OtherRates = otherRates,
                    ZoneId = supplierZone.SupplierZoneId,
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
