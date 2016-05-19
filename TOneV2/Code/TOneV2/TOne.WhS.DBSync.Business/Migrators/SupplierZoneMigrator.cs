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
    public class SupplierZoneMigrator : Migrator<SourceZone, SupplierZone>
    {
        SupplierZoneDBSyncDataManager dbSyncDataManager;
        SourceZoneDataManager dataManager;

        public SupplierZoneMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SupplierZoneDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceZoneDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<SupplierZone> itemsToAdd)
        {
            long startingId;
            ReserveIDRange(itemsToAdd.Count(), out startingId);
            dbSyncDataManager.ApplySupplierZonesToTemp(itemsToAdd, startingId);
            DBTable dbTableSupplierZone = Context.DBTables[DBTableName.SupplierZone];
            if (dbTableSupplierZone != null)
                dbTableSupplierZone.Records = dbSyncDataManager.GetSupplierZones();
        }

        public override IEnumerable<SourceZone> GetSourceItems()
        {
            return dataManager.GetSourceZones(false);
        }

        public override SupplierZone BuildItemFromSource(SourceZone sourceItem)
        {
            DBTable dbTableCarrierAccount = Context.DBTables[DBTableName.CarrierAccount];
            DBTable dbTableCountry = Context.DBTables[DBTableName.Country];
            if (dbTableCountry != null && dbTableCarrierAccount != null)
            {
                Dictionary<string, CarrierAccount> allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;
                CarrierAccount carrierAccount = null;
                if (allCarrierAccounts != null)
                    allCarrierAccounts.TryGetValue(sourceItem.SupplierId, out carrierAccount);

                Dictionary<string, Country> allCountries = (Dictionary<string, Country>)dbTableCountry.Records;
                Country country = null;
                if (allCountries != null)
                    allCountries.TryGetValue(sourceItem.CodeGroup, out country);

                if (country != null && carrierAccount != null)
                    return new SupplierZone
                    {
                        SupplierId = carrierAccount.CarrierAccountId,
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
            IDManager.Instance.ReserveIDRange(typeof(SupplierZoneManager), nbOfIds, out startingId);
        }
    }
}
