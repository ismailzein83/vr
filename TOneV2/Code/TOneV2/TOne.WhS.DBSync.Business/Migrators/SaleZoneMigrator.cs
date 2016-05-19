using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SaleZoneMigrator : Migrator<SourceZone, SaleZone>
    {
        SaleZoneDBSyncDataManager dbSyncDataManager;
        SourceZoneDataManager dataManager;

        public SaleZoneMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SaleZoneDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceZoneDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<SaleZone> itemsToAdd)
        {
            long startingId;
            ReserveIDRange(itemsToAdd.Count(), out startingId);
            dbSyncDataManager.ApplySaleZonesToTemp(itemsToAdd, startingId);
            DBTable dbTableSaleZone = Context.DBTables[DBTableName.SaleZone];
            if (dbTableSaleZone != null)
                dbTableSaleZone.Records = dbSyncDataManager.GetSaleZones();
        }

        public override IEnumerable<SourceZone> GetSourceItems()
        {
            return dataManager.GetSourceZones(true);
        }

        public override SaleZone BuildItemFromSource(SourceZone sourceItem)
        {
            DBTable dbTableCountry = Context.DBTables[DBTableName.Country];
            if (dbTableCountry != null)
            {
                Dictionary<string, Country> allCountries = (Dictionary<string, Country>)dbTableCountry.Records;
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
                    return null;

            }
            else
                return null;
        }

        internal static void ReserveIDRange(int nbOfIds, out long startingId)
        {
            IDManager.Instance.ReserveIDRange(typeof(SaleZoneManager), nbOfIds, out startingId);
        }
    }
}
