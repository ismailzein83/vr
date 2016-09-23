using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class VRTimeZoneMigrator : Migrator<SourceCustomTimeZoneInfo, VRTimeZone>
    {
        VRTimeZoneDBSyncDataManager dbSyncDataManager;
        SourceCustomTimeZoneInfoDataManager dataManager;

        public VRTimeZoneMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new VRTimeZoneDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCustomTimeZoneInfoDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate(MigrationInfoContext context)
        {
            VRTimeZoneManager manager = new VRTimeZoneManager();
            context.GeneratedIdsInfoContext = new GeneratedIdsInfoContext();
            context.GeneratedIdsInfoContext.TypeId = manager.GetVRTimeZoneTypeId();
            base.Migrate(context);
        }

        public override void AddItems(List<VRTimeZone> itemsToAdd)
        {
            dbSyncDataManager.ApplyTimeZonesToTemp(itemsToAdd, 1);
            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceCustomTimeZoneInfo> GetSourceItems()
        {
            return dataManager.GetSourceCustomTimeZonesInfo();
        }

        public override VRTimeZone BuildItemFromSource(SourceCustomTimeZoneInfo sourceItem)
        {
            VRTimeZoneSettings timeZoneSettings = new VRTimeZoneSettings();

            timeZoneSettings.Offset = TimeSpan.FromMinutes(sourceItem.BaseUtcOffset);

            return new VRTimeZone
            {
                Name = sourceItem.DisplayName,
                Settings = timeZoneSettings,
                SourceId = sourceItem.SourceId
            };
        }

        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableVRTimeZone = Context.DBTables[DBTableName.VRTimeZone];
            if (dbTableVRTimeZone != null)
                dbTableVRTimeZone.Records = dbSyncDataManager.GetVrTimeZones(useTempTables);
        }

    }
}
