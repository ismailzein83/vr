using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SourceSwitchMigrator : Migrator<SourceSwitch, Switch>
    {
        SwitchDBSyncDataManager dbSyncDataManager;
        SourceSwitchDataManager dataManager;

        public SourceSwitchMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SwitchDBSyncDataManager(context.UseTempTables);
            dataManager = new SourceSwitchDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<Switch> itemsToAdd)
        {
            dbSyncDataManager.ApplySwitchesToTemp(itemsToAdd);
        }

        public override IEnumerable<SourceSwitch> GetSourceItems()
        {
            return dataManager.GetSourceSwitches();
        }

        public override Switch BuildItemFromSource(SourceSwitch sourceItem)
        {
            return new Switch
            {
                Name = sourceItem.Name,
                SourceId = sourceItem.SourceId
            };
        }
    }
}
