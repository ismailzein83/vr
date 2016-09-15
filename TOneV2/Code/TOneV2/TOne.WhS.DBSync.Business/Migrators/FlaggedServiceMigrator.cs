using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class FlaggedServiceMigrator : Migrator<SourceFlaggedService, ZoneServiceConfig>
    {
        SourceFlaggedServiceDataManager dataManager;
        ZoneServiceConfigDBSyncDataManager dbSyncDataManager;

        public FlaggedServiceMigrator(MigrationContext context)
            : base(context)
        {
            dataManager = new SourceFlaggedServiceDataManager(Context.ConnectionString);
            dbSyncDataManager = new ZoneServiceConfigDBSyncDataManager(context.UseTempTables);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate(MigrationInfoContext context)
        {
            base.Migrate(context);
        }

        public override void AddItems(List<ZoneServiceConfig> itemsToAdd)
        {
            dbSyncDataManager.ApplyZoneServicesConfigToTemp(itemsToAdd, 1);
            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceFlaggedService> GetSourceItems()
        {
            return dataManager.GetSourceFlaggedServices();
        }

        public override ZoneServiceConfig BuildItemFromSource(SourceFlaggedService sourceItem)
        {
            ServiceConfigSetting serviceConfigSetting = new ServiceConfigSetting()
            {
                Color = sourceItem.ServiceColor,
                Name = sourceItem.Name,
                Description = sourceItem.Description
            };

            return new ZoneServiceConfig
            {
                Symbol = sourceItem.Symbol,
                Settings = serviceConfigSetting,
                SourceId = sourceItem.SourceId
            };
        }

        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableZoneServiceConfig = Context.DBTables[DBTableName.ZoneServiceConfig];
            if (dbTableZoneServiceConfig != null)
                dbTableZoneServiceConfig.Records = dbSyncDataManager.GetZoneServicesConfig(useTempTables);

        }
    }
}
