using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SwitchMigrator : Migrator<SourceSwitch, Switch>
    {
        SwitchDBSyncDataManager dbSyncDataManager;
        SourceSwitchDataManager dataManager;

        public SwitchMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SwitchDBSyncDataManager(Context.UseTempTables);
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
            TotalRows = itemsToAdd.Count;
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
        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableSwitch = Context.DBTables[DBTableName.Switch];
            if (dbTableSwitch != null)
                dbTableSwitch.Records = dbSyncDataManager.GetSwitches(useTempTables);
        }
    }
}
