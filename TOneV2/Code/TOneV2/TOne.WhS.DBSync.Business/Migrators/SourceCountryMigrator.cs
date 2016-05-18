using System.Collections.Generic;
using System.Linq;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SourceCountryMigrator : Migrator<TOne.WhS.DBSync.Entities.SourceCountry, Country>
    {
        CountryDBSyncDataManager dbSyncDataManager;
        SourceCountryDataManager dataManager;

        public SourceCountryMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CountryDBSyncDataManager(context.UseTempTables);
            dataManager = new SourceCountryDataManager(Context.ConnectionString);
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
            DBTable dbTableCountry = Context.DBTables[DBTableName.Country];
            if (dbTableCountry != null)
                dbTableCountry.Records = dbSyncDataManager.GetCountries();
        }

        public override IEnumerable<TOne.WhS.DBSync.Entities.SourceCountry> GetSourceItems()
        {
            return dataManager.GetSourceCountries();
        }

        public override Country BuildItemFromSource(TOne.WhS.DBSync.Entities.SourceCountry sourceItem)
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
    }
}
