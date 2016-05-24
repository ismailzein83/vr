﻿using System.Collections.Generic;
using System.Linq;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class CountryMigrator : Migrator<SourceCodeGroup, Country>
    {
        CountryDBSyncDataManager dbSyncDataManager;
        SourceCodeGroupDataManager dataManager;

        public CountryMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CountryDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCodeGroupDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<Country> itemsToAdd)
        {
            long startingId;
            ReserveIDRange(itemsToAdd.Count(), out startingId);
            dbSyncDataManager.ApplyCountriesToTemp(itemsToAdd, (int)startingId);
        }

        public override IEnumerable<SourceCodeGroup> GetSourceItems()
        {
            return dataManager.GetSourceCodeGroups();
        }

        public override Country BuildItemFromSource(SourceCodeGroup sourceItem)
        {
            return new Country
            {
                Name = sourceItem.Name,
                SourceId = sourceItem.SourceId
            };
        }

        internal static void ReserveIDRange(int nbOfIds, out long startingId)
        {
            IDManager.Instance.ReserveIDRange(typeof(CountryManager), nbOfIds, out startingId);
        }

        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableCountry = Context.DBTables[DBTableName.Country];
            if (dbTableCountry != null)
                dbTableCountry.Records = dbSyncDataManager.GetCountries(useTempTables);
        }
    }
}
