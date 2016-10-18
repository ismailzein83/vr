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

        static Dictionary<string, string> S_ServiceColors = new Dictionary<string, string>();

        public FlaggedServiceMigrator(MigrationContext context)
            : base(context)
        {
            dataManager = new SourceFlaggedServiceDataManager(Context.ConnectionString);
            dbSyncDataManager = new ZoneServiceConfigDBSyncDataManager(context.UseTempTables);
            TableName = dbSyncDataManager.GetTableName();
            FillServiceColors(S_ServiceColors);

        }

        private void FillServiceColors(Dictionary<string, string> S_ServiceColors)
        {
            if (S_ServiceColors.Count > 0)
                return;

            S_ServiceColors.Add("WHS", "#C0C0C0");
            S_ServiceColors.Add("RTL", "#0000FF");
            S_ServiceColors.Add("PRM", "#FFA500");
            S_ServiceColors.Add("VID", "#800000");
            S_ServiceColors.Add("PRC", "#800000");
            S_ServiceColors.Add("CLI", "#FF0000");
            S_ServiceColors.Add("DRC", "#00FF00");
            S_ServiceColors.Add("DUM", "#00FF00");
            S_ServiceColors.Add("TRS", "#FFFF00");
            S_ServiceColors.Add("3GM", "#000000");
            S_ServiceColors.Add("GRY", "#000000");
            S_ServiceColors.Add("ALG", "#FFFF00");
            S_ServiceColors.Add("TDM", "#FFFF00");
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
                Color = sourceItem.ServiceColor != null ? string.Concat("#", sourceItem.ServiceColor) : S_ServiceColors[sourceItem.Symbol],
                Name = sourceItem.Name,
                Description = sourceItem.Description,
                Weight = int.Parse(sourceItem.SourceId)
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
