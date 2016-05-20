﻿using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SaleRateMigrator : Migrator<SourceRate, SaleRate>
    {
        SaleRateDBSyncDataManager dbSyncDataManager;
        SourceRateDataManager dataManager;
        DBTable dbTableSaleZone;
        DBTable dbTableSalePriceList;
        DBTable dbTableCurrency;

        public SaleRateMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SaleRateDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceRateDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            dbTableSaleZone = Context.DBTables[DBTableName.SaleZone];
            dbTableSalePriceList = Context.DBTables[DBTableName.SalePriceList];
            dbTableCurrency = Context.DBTables[DBTableName.Currency];
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<SaleRate> itemsToAdd)
        {
            dbSyncDataManager.ApplySaleRatesToTemp(itemsToAdd);
        }

        public override IEnumerable<SourceRate> GetSourceItems()
        {
            return dataManager.GetSourceRates(true);
        }

        public override SaleRate BuildItemFromSource(SourceRate sourceItem)
        {
            if (dbTableCurrency != null && dbTableSalePriceList != null && dbTableSaleZone != null)
            {
                var allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZone.Records;
                Dictionary<string, SalePriceList> allSalePriceLists = (Dictionary<string, SalePriceList>)dbTableSalePriceList.Records;
                var allCurrencies = (Dictionary<string, Currency>)dbTableCurrency.Records;
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
                    return null;

            }
            else
                return null;
        }

    }
}
