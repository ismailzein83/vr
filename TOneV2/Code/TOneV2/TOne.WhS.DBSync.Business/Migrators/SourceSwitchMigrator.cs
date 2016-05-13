using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Business.EntityMigrator;
using TOne.WhS.DBSync.Business.SourceMigratorsReaders;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SourceSwitchMigrator : SourceItemMigrator<SourceSwitch, Switch, SourceSwitchMigratorReader>
    {
        bool _UseTempTables;
        DBSyncLogger _Logger;

        public SourceSwitchMigrator(SourceSwitchMigratorReader sourceSwitchMigratorReader, bool useTempTables, DBSyncLogger logger)
            : base(sourceSwitchMigratorReader)
        {
            _UseTempTables = useTempTables;
            _Logger = logger;
        }


        public override void Migrate(List<DBTable> context)
        {
            _Logger.WriteInformation("Migrating table 'Switch' started");
            base.Migrate(context);
            _Logger.WriteInformation("Migrating table 'Switch' ended");
        }

        protected override void AddItems(List<Switch> itemsToAdd, List<DBTable> context)
        {
            SwitchDBSyncManager switchManager = new SwitchDBSyncManager(_UseTempTables);
            switchManager.ApplySwitchesToTemp(itemsToAdd);
        }

        protected override Switch BuildItemFromSource(SourceSwitch sourceItem, List<DBTable> context)
        {
            return new Switch
            {
                Name = sourceItem.Name,
                SourceId = sourceItem.SourceId
            };
        }
    }
}
