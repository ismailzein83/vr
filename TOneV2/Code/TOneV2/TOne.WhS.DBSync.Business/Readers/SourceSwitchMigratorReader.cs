using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities.EntityMigrator;

namespace TOne.WhS.DBSync.Business.SourceMigratorsReaders
{
    public class SourceSwitchMigratorReader : IMigrationSourceItemReader<SourceSwitch>
    {
        private string _ConnectionString;

        public SourceSwitchMigratorReader(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        public IEnumerable<SourceSwitch> GetSourceItems()
        {
            SourceSwitchDataManager dataManager = new SourceSwitchDataManager(_ConnectionString);
            return dataManager.GetSourceSwitches();
        }
    }
}
