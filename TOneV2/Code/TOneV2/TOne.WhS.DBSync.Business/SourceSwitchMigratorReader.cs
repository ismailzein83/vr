using System.Collections.Generic;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities.EntityMigrator;

namespace TOne.WhS.DBSync.Business.SourceMigratorsReaders
{
    public class SourceSwitchMigratorReader : IMigrationSourceItemReader<SourceSwitch> 
    {
        public string ConnectionString { get; set; }

        public IEnumerable<SourceSwitch> GetSourceItems()
        {
            DataManager dataManager = new DataManager(this.ConnectionString);
            return dataManager.GetSourceSwitches();
        }

        private class DataManager : Vanrise.Data.SQL.BaseSQLDataManager
        {
            public DataManager(string connectionString)
                : base(connectionString, false)
            {
            }

            public List<SourceSwitch> GetSourceSwitches()
            {
                return GetItemsText(query_getSourceSwitches, SourceSwitchMapper, null);
            }

            private SourceSwitch SourceSwitchMapper(System.Data.IDataReader arg)
            {
                SourceSwitch sourceSwitch = new SourceSwitch()
                {
                    SourceId = arg["SwitchID"].ToString(),
                    Name = arg["Name"].ToString()
                };
                return sourceSwitch;
            }

            const string query_getSourceSwitches = @"SELECT [SwitchID] ,[Name] FROM [dbo].[Switch]";
        }
    }
}
