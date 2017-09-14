﻿using System;
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
        int _offPeakRateTypeId;
        int _weekendRateTypeId;
        bool _onlyEffective;
        public SupplierRateMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SupplierRateDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceRateDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableSupplierZone = Context.DBTables[DBTableName.SupplierZone];
            var dbTableSupplierPriceList = Context.DBTables[DBTableName.SupplierPriceList];
            allSupplierZones = (Dictionary<string, SupplierZone>)dbTableSupplierZone.Records;
            allSupplierPriceLists = (Dictionary<string, SupplierPriceList>)dbTableSupplierPriceList.Records;
            _offPeakRateTypeId = context.OffPeakRateTypeId;
            _weekendRateTypeId = context.WeekendRateTypeId;
            _onlyEffective = context.OnlyEffective;
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
            dbSyncDataManager.ApplySupplierRatesToTemp(itemsToAdd, TotalRowsSuccess + 1);
            TotalRowsSuccess = TotalRowsSuccess + itemsToAdd.Count;
        }

        public override IEnumerable<SourceRate> GetSourceItems()
        {
            return dataManager.GetSourceRates(false, _onlyEffective);
        }

        public override SupplierRate BuildItemFromSource(SourceRate sourceItem)
        {
            SupplierZone supplierZone = null;
            SupplierPriceList supplierPriceList = null;
            if (allSupplierPriceLists != null && sourceItem.PriceListId.HasValue)
                allSupplierPriceLists.TryGetValue(sourceItem.PriceListId.Value.ToString(), out supplierPriceList);

            if (allSupplierZones != null && sourceItem.ZoneId.HasValue)
                allSupplierZones.TryGetValue(sourceItem.ZoneId.Value.ToString(), out supplierZone);

            int? rateTypeId = null;
             if (sourceItem.RateType.HasValue &&  sourceItem.RateType == RateTypeEnum.OffPeak)
                rateTypeId = _offPeakRateTypeId;
             else if (sourceItem.RateType.HasValue && sourceItem.RateType == RateTypeEnum.Weekend)
                rateTypeId = _weekendRateTypeId;

            if (supplierZone != null && supplierPriceList != null && sourceItem.BeginEffectiveDate.HasValue && sourceItem.Rate.HasValue)
                return new SupplierRate
                {
                    BED = sourceItem.BeginEffectiveDate.Value,
                    EED = sourceItem.EndEffectiveDate,
                    Rate = sourceItem.Rate.Value,
                    PriceListId = supplierPriceList.PriceListId,
                    ZoneId = supplierZone.SupplierZoneId,
                    RateTypeId = rateTypeId,
                    RateChange = GetRateChangeType(sourceItem.Change.Value),
                    SourceId = sourceItem.SourceId
                };
            else
            {
                TotalRowsFailed++;
                Context.WriteWarning(string.Format("Failed migrating Supplier Rate, Source Id: {0}", sourceItem.SourceId));

                return null;
            }
        }


        public override void FillTableInfo(bool useTempTables)
        {

        }

        public override void LoadSourceItems(Action<SourceRate> onItemLoaded)
        {
            dataManager.LoadSourceItems(false,_onlyEffective, onItemLoaded);
        }

        public override bool IsLoadItemsApproach
        {
            get
            {
                return true;
            }
        }

        private SupplierRate SupplierRateMapper(SourceRate sourceItem, decimal? rate, int? rateTypeId, int currencyId, int priceListId, long supplierZoneId)
        {
            return new SupplierRate()
            {
                BED = sourceItem.BeginEffectiveDate.Value,
                EED = sourceItem.EndEffectiveDate,
                Rate = rate.Value,
                CurrencyId = currencyId,
                PriceListId = priceListId,
                ZoneId = supplierZoneId,
                RateTypeId = rateTypeId,
                RateChange = GetRateChangeType(sourceItem.Change.Value),
                SourceId = sourceItem.SourceId
            };
        }

        private RateChangeType GetRateChangeType(Int16 sourceRateChangeType)
        {
            RateChangeType result = RateChangeType.NotChanged;
            switch (sourceRateChangeType)
            {
                case -1:
                    result = RateChangeType.Decrease;
                    break;
                case 0:
                    result = RateChangeType.NotChanged;
                    break;
                case 1:
                    result = RateChangeType.Increase;
                    break;
                case 2:
                    result = RateChangeType.New;
                    break;
            }
            return result;

        }

    }

}
