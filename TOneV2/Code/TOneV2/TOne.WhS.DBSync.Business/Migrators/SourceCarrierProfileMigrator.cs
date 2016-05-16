using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SourceCarrierProfileMigrator : Migrator
    {
        public SourceCarrierProfileMigrator(string connectionString, bool useTempTables, DBSyncLogger logger)
            : base(connectionString, useTempTables, logger)
        {
        }

        public override void Migrate(List<DBTable> context)
        {
            Logger.WriteInformation("Migrating table 'CarrierProfile' started");
            var sourceItems = GetSourceItems();
            if (sourceItems != null)
            {
                List<CarrierProfile> itemsToAdd = new List<CarrierProfile>();
                foreach (var sourceItem in sourceItems)
                {
                    var item = BuildItemFromSource(sourceItem, context);
                    if (item != null)
                        itemsToAdd.Add(item);
                }
                AddItems(itemsToAdd, context);
            }
            Logger.WriteInformation("Migrating table 'CarrierProfile' ended");
        }

        private  void AddItems(List<CarrierProfile> itemsToAdd, List<DBTable> context)
        {
            CarrierProfileDBSyncManager CarrierProfileManager = new CarrierProfileDBSyncManager(UseTempTables);
            CarrierProfileManager.ApplyCarrierProfilesToTemp(itemsToAdd);
            DBTable dbTableCarrierProfile = context.Where(x => x.Name == Constants.Table_CarrierProfile).FirstOrDefault();
            if (dbTableCarrierProfile != null)
                dbTableCarrierProfile.Records = CarrierProfileManager.GetCarrierProfiles();
        }

        private IEnumerable<SourceCarrierProfile> GetSourceItems()
        {
            SourceCarrierProfileDataManager dataManager = new SourceCarrierProfileDataManager(ConnectionString);
            return dataManager.GetSourceCarrierProfiles();
        }

        private  CarrierProfile BuildItemFromSource(SourceCarrierProfile sourceItem, List<DBTable> context)
        {
            return new CarrierProfile
            {
                Name = sourceItem.Name,
                SourceId = sourceItem.SourceId
            };
        }
    }
}
