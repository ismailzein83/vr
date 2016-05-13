using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SourceSwitchMigrator 
    {
        private string _ConnectionString;
        private bool _UseTempTables;
        private DBSyncLogger _Logger;

        public SourceSwitchMigrator(string connectionString, bool useTempTables, DBSyncLogger logger)
        {
            _UseTempTables = useTempTables;
            _Logger = logger;
            _ConnectionString = connectionString;
        }


        public void Migrate(List<DBTable> context)
        {
            _Logger.WriteInformation("Migrating table 'Switch' started");
            var sourceItems = GetSourceItems();
            if (sourceItems != null)
            {
                List<Switch> itemsToAdd = new List<Switch>();
                foreach (var sourceItem in sourceItems)
                {
                    var item = BuildItemFromSource(sourceItem, context);
                    if (item != null)
                        itemsToAdd.Add(item);
                }
                AddItems(itemsToAdd, context);
            }
            _Logger.WriteInformation("Migrating table 'Switch' ended");
        }

        private void AddItems(List<Switch> itemsToAdd, List<DBTable> context)
        {
            SwitchDBSyncManager switchManager = new SwitchDBSyncManager(_UseTempTables);
            switchManager.ApplySwitchesToTemp(itemsToAdd);
        }

        private IEnumerable<SourceSwitch> GetSourceItems()
        {
            SourceSwitchDataManager dataManager = new SourceSwitchDataManager(_ConnectionString);
            return dataManager.GetSourceSwitches();
        }

        private Switch BuildItemFromSource(SourceSwitch sourceItem, List<DBTable> context)
        {
            return new Switch
            {
                Name = sourceItem.Name,
                SourceId = sourceItem.SourceId
            };
        }
    }
}
