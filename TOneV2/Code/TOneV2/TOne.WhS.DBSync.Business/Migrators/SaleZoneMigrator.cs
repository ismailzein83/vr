﻿using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SaleZoneMigrator : Migrator<SourceZone, SaleZone>
    {
        SaleZoneDBSyncDataManager dbSyncDataManager;
        SourceZoneDataManager dataManager;
        Dictionary<string, Country> allCountries;

        public SaleZoneMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SaleZoneDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceZoneDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableCountry = Context.DBTables[DBTableName.Country];
            allCountries = (Dictionary<string, Country>)dbTableCountry.Records;
        }

        public override void Migrate(MigrationInfoContext context)
        {
            SaleZoneManager manager = new SaleZoneManager();
            context.GeneratedIdsInfoContext = new GeneratedIdsInfoContext();
            context.GeneratedIdsInfoContext.TypeId = manager.GetSaleZoneTypeId();
            base.Migrate(context);
        }

        public override void AddItems(List<SaleZone> itemsToAdd)
        {
            dbSyncDataManager.ApplySaleZonesToTemp(itemsToAdd, 1);
            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceZone> GetSourceItems()
        {
            return dataManager.GetSourceZones(true);
        }

        public override SaleZone BuildItemFromSource(SourceZone sourceItem)
        {
            Country country = null;
            if (allCountries != null)
                allCountries.TryGetValue(sourceItem.CodeGroup, out country);
            if (country != null)
                return new SaleZone
                    {
                        SellingNumberPlanId = Context.DefaultSellingNumberPlanId,
                        BED = sourceItem.BED,
                        CountryId = country.CountryId,
                        EED = sourceItem.EED,
                        Name = sourceItem.Name,
                        SourceId = sourceItem.SourceId
                    };
            else
            {
                TotalRowsFailed++;
                return null;
            }
        }

        internal static void ReserveIDRange(int nbOfIds, out long startingId)
        {
            IDManager.Instance.ReserveIDRange(typeof(SaleZoneManager), nbOfIds, out startingId);
        }
        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableSaleZone = Context.DBTables[DBTableName.SaleZone];
            if (dbTableSaleZone != null)
                dbTableSaleZone.Records = dbSyncDataManager.GetSaleZones(useTempTables);
        }
    }
}
