using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SourceCarrierProfileMigrator : Migrator<SourceCarrierProfile, CarrierProfile>
    {
        CarrierProfileDBSyncDataManager dbSyncDataManager;
        SourceCarrierProfileDataManager dataManager;

        public SourceCarrierProfileMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CarrierProfileDBSyncDataManager(context.UseTempTables);
            dataManager = new SourceCarrierProfileDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<CarrierProfile> itemsToAdd)
        {
            dbSyncDataManager.ApplyCarrierProfilesToTemp(itemsToAdd);
            DBTable dbTableCarrierProfile = Context.DBTables[DBTableName.CarrierProfile];
            if (dbTableCarrierProfile != null)
                dbTableCarrierProfile.Records = dbSyncDataManager.GetCarrierProfiles();
        }

        public override IEnumerable<SourceCarrierProfile> GetSourceItems()
        {
            return dataManager.GetSourceCarrierProfiles();
        }

        public override CarrierProfile BuildItemFromSource(SourceCarrierProfile sourceItem)
        {
            CarrierProfileSettings settings = new CarrierProfileSettings();
            //settings.Address = sourceItem.Address1 + " " + sourceItem.Address2 + " " + sourceItem.Address3;

            return new CarrierProfile
            {
                Name = sourceItem.Name,
                SourceId = sourceItem.SourceId,
                Settings = settings
            };
        }
    }
}
